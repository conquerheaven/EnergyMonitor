import sqlserver_connector
import datetime
import DTW
import Interpolate
def str_to_datetime(s):
    return datetime.datetime.strptime(s, '%Y-%m-%d %H:%M:%S')

'''
根据测点ID，时间区间(startTime , entTime)，返回非0能耗值list
'''
def getDataByNoAndTime(AnalogNo , startTime , endTime):
    mmsql = sqlserver_connector.MSSQL(host = "localhost" , user = "sa" , pwd = "123456" , db = "EnergyMonitor")
    sql = 'select AHH_Value from AnalogHistoryHour where AHH_AnalogNo = %s and AHH_HTime > \'%s\' and AHH_HTime < \'%s\'' % (AnalogNo , startTime , endTime)
    list = mmsql.ExecQuery(sql)
    result = []
    for t in list:
        result.append(t[0])
    return result

'''
获取所有测点ID
'''
def getAllAnalogNo():
    mmsql = sqlserver_connector.MSSQL(host = "localhost" , user = "sa" , pwd = "123456" , db = "EnergyMonitor")
    sql = 'select distinct(AHH_AnalogNo) from AnalogHistoryHour'
    list = mmsql.ExecQuery(sql)
    result = []
    for t in list:
        result.append(t[0])
    return result

'''
返回list尾部连续非0序列
'''
def getConsecutivePreData(preData):
    result = []
    l = len(preData)-1
    while l >= 0:
        if preData[l] > 0: result.append(preData[l])
        else: break
        l -= 1
    result.reverse()
    return result

'''
返回list头部连续非0序列
'''
def getConsecutivePostData(postData):
    result = []
    l = 0
    while l < len(postData):
        if postData[l] > 0: result.append(postData[l])
        else: break
        l += 1
    return result

'''
判断list是否全部非0
'''
def isAllHasValue(dataList):
    for d in dataList:
        if d <= 0: return False
    return True

'''
将递增能耗曲线转换为非递增能耗曲线，并返回两条曲线相似度
'''
def getSimilar(datalist1 , datalist2):
    len1 = len(datalist1)
    len2 = len(datalist2)
    sum1 = datalist1[len1-1] - datalist1[0]
    sum2 = datalist2[len2-1] - datalist2[0]
    if sum1 == 0:
        print('sum1=0 %s %s' % (datalist1[0] , datalist1[len1-1]))
        print(datalist1)
        return 10000
    if sum2 == 0:
        print('sum2=0 %s %s' % (datalist2[0] , datalist2[len2-1]))
        print(datalist2)
        return 10000

    processdata1 = []
    processdata2 = []
    n = 1
    while n < len1:
        if not (datalist1[n] <= 0 or datalist1[n-1] <= 0):
            processdata1.append((datalist1[n]-datalist1[n-1])*100/sum1)
        n += 1

    #print(n)
    n = 1
    while n < len2:
        if not (datalist2[n] <= 0 or datalist2[n-1] <= 0):
            processdata2.append((datalist2[n]-datalist2[n-1])*100/sum2)
        n += 1
    #print(n)
    dtw = DTW.DTW()
    return dtw.similarity(processdata1 , processdata2)
'''
将能耗读数，压缩为百分比
'''
def CompressData(data):
    l = len(data)
    sum = data[l-1]-data[0]
    n = 1
    result = []
    while n < l:
        if not (data[n] <= 0 or data[n-1] <= 0):
            result.append((data[n]-data[n-1])*100/sum)
        n += 1
    return result

def getConsumption(data):
    result = []
    for i in range(len(data)):
        if i == 0: continue
        result.append((data[i]-data[i-1]))
    return result

def Knn():
    AnalogNoList = getAllAnalogNo()
    fo = open('missingGap' , 'r')
    fw = open('KnnResult' , 'w')
    AnalogNo = fo.readline().strip()
    while AnalogNo:
        startTime = str_to_datetime(fo.readline().strip())
        endTime = str_to_datetime(fo.readline().strip())
        preWeekData = getDataByNoAndTime(AnalogNo , startTime+datetime.timedelta(days = -7) , startTime)#获取前一周数据
        preWeekData = getConsecutivePreData(preWeekData)#获得前一周连续非0数据
        postWeekData = getDataByNoAndTime(AnalogNo , endTime , endTime+datetime.timedelta(days = 7))#获取后一周数据
        postWeekData = getConsecutivePostData(postWeekData)#获取后一周连续非0数据
        print(AnalogNo , startTime , endTime , '\n')
        print(preWeekData , '\n')
        print(postWeekData , '\n')
        similarList = []
        #print(preWeekData)

        for id in AnalogNoList:
            if int(id) == int(AnalogNo): continue

            tempPreWeekData = getDataByNoAndTime(id , startTime+datetime.timedelta(hours = -1*len(preWeekData)-1) , startTime)#获取前一周与AnalogNo长度相等的数据
            tempPostWeekData = getDataByNoAndTime(id , endTime , endTime+datetime.timedelta(hours = len(postWeekData)+1))#获取后一周与AnalogNo长度相等的数据
            midData = getDataByNoAndTime(id , startTime+datetime.timedelta(hours = -2) , endTime+datetime.timedelta(hours = 2))

            #print(AnalogNo , id , '\n')
            #print(tempPreWeekData , '\n')
            #print(tempPostWeekData , '\n')
            if len(tempPreWeekData) == 0 or len(tempPostWeekData) == 0 or len(midData) == 0: continue#改测点前一周、后一周和中间部分的必须有数据
            if isAllHasValue(midData) == False: continue#中间部分不能有缺失数据
            if tempPreWeekData[0] == 0 or tempPostWeekData[0] == 0: continue#前一周、后一周的数据的两个边界不能缺失，否则无法正确计算百分比
            if tempPreWeekData[len(tempPreWeekData)-1] == 0 or tempPostWeekData[len(tempPostWeekData)-1] == 0: continue#前一周、后一周的数据的两个边界不能缺失，否则无法正确计算百分比

            tstart = datetime.datetime.now()
            similar = (getSimilar(preWeekData , tempPreWeekData) + getSimilar(postWeekData , tempPostWeekData))/2
            tend = datetime.datetime.now()
            #print('计算%s与%s相似度 耗时：%s' % (AnalogNo , id , tend-tstart))
            similarList.append((AnalogNo , id , similar))
            #fo.writelines('%s %s %s' % (AnalogNo , id , similar))
        similarList = sorted(similarList , key = lambda s:s[2])

        print(similarList[0])
        print(similarList[1])
        print(similarList[2])
        k = 3#获取前k条最相似的时间序列曲线
        lineList = []
        for i in range(k):
            midData = getDataByNoAndTime(similarList[i][1] , startTime+datetime.timedelta(hours = -2) , endTime+datetime.timedelta(hours = 2))#获取前k条相似序列对应缺失部分的数据
            processData = CompressData(midData)#压缩成百分比
            lineList.append(processData)
        ipl = Interpolate.Interpolate()
        x = ipl.interpolate(lineList)#拉格朗日乘数法，得到缺失插补值
        midData = getDataByNoAndTime(AnalogNo , startTime+datetime.timedelta(hours = -2) , endTime+datetime.timedelta(hours = 2))
        X = CompressData(midData)#得到缺失数据真实值

        ##输出比例
        msePercent = 0
        for i in range(len(x)):
            fw.write('%.8f'%x[i])
            fw.write('  ')
        fw.write('\n')
        for i in range(len(X)):
            fw.write('%.8f'%X[i])
            fw.write('  ')
            msePercent += (X[i]-x[i])*(X[i]-x[i])
        fw.write('\n')
        fw.write('\n')
        ##输出能耗数值
        mseValue = 0
        sum = midData[len(midData)-1] - midData[0]
        for i in range(len(x)):
            fw.write('%.8f'%(x[i]*sum/100.0))
            fw.write('  ')
        fw.write('\n')
        for i in range(len(X)):
            fw.write('%.8f'%(X[i]*sum/100.0))
            fw.write('  ')
            mseValue += (X[i]*sum/100.0-x[i]*sum/100.0)*(X[i]*sum/100.0-x[i]*sum/100.0)
        fw.write('\n')
        fw.write('\n')

        print(AnalogNo , startTime , endTime , "插补值", x)
        print(AnalogNo , startTime , endTime , "真实值", X)
        print("msePercent = %s , mseValue = %s"%(msePercent/len(x) , mseValue/len(x)))
        print('\n')
        AnalogNo = fo.readline().strip()
    fo.close()
    fw.close()

def L(x , fx):
    r = 0
    for i in range(len(fx)):
        li = 1.0
        for j in range(len(fx)):
            if i == j: continue
            li = li*(x-fx[j][0])/(fx[i][0]-fx[j][0])
        print(li)
        r = r + fx[i][1]*li
    return r

def Lagrange():
    fo = open('missingGap' , 'r')
    fw = open('LagrangeResult' , 'w')
    AnalogNo = fo.readline().strip()
    while AnalogNo:
        startTime = str_to_datetime(fo.readline().strip())
        endTime = str_to_datetime(fo.readline().strip())
        timegap = (endTime-startTime).days*24 + int((endTime - startTime).seconds/3600) + 2
        #print(timegap)

        preWeekData = getDataByNoAndTime(AnalogNo , startTime+datetime.timedelta(days = -7) , startTime)#获取前一周数据
        preWeekData = getConsecutivePreData(preWeekData)#获得前一周连续非0数据
        Y1 = getConsumption(preWeekData)

        postWeekData = getDataByNoAndTime(AnalogNo , endTime , endTime+datetime.timedelta(days = 7))#获取后一周数据
        postWeekData = getConsecutivePostData(postWeekData)#获取后一周连续非0数据
        Y2 = getConsumption(postWeekData)

        #print(Y1)
        #print(Y2)

        fx = []
        for i in range(len(Y1)):
            fx.append([i , Y1[i]])
        for i in range(len(Y2)):
            fx.append([i+len(Y1)+timegap , Y2[i]])
        print(fx)
        result = []
        for i in range(timegap):
            x = i+len(Y1)
            result.append(L(x , fx))

        midData = getDataByNoAndTime(AnalogNo , startTime+datetime.timedelta(hours = -2) , endTime+datetime.timedelta(hours = 2))
        realData = getConsumption(midData)#得到缺失数据真实值

        for i in range(len(result)):
            fw.write('%.8f'%(result[i]))
            fw.write('  ')
        fw.write('\n')
        for i in range(len(realData)):
            fw.write('%.8f'%(realData[i]))
            fw.write('  ')
        fw.write('\n')
        fw.write('\n')

        AnalogNo = fo.readline().strip()
    fo.close()
    fw.close()

if __name__ == '__main__':
    #print((str_to_datetime('2017-07-09 11:00:00')-str_to_datetime('2017-07-08 11:00:00')).seconds)
    #Knn()
    Lagrange()
