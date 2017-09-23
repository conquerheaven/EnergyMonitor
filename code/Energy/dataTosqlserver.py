import datetime
import sqlserver_connector
import downloadData
import json

def str_to_datetime(s):
    return datetime.datetime.strptime(s, '%Y-%m-%d %H:%M:%S')

class Main(object):

    def __init__(self , analogNo , startTime , endTime , granularity):
        self.analogNo = analogNo
        self.startTime = startTime
        self.endTime = endTime
        self.granularity = granularity

    def readinput(self):
        self.analogNo = 200579 #input('analogNo:')
        self.startTime = str_to_datetime('2016-11-11 00:00:00') #(input('starTime:'))
        self.endTime = str_to_datetime('2016-12-01 01:23:23') #(input('endTime:'))
        self.granularity = 1 #input('granularity:')

    def computing(self):
        download = downloadData.DownloadData(self.analogNo , self.startTime , self.endTime , self.granularity)
        result = download.fetchData()
        historyData = []
        if result[0] == True:
            jsondata = result[1]
            for data in jsondata['data']:
                #print ('%s %s %s %s' % (data['PNO'],data['Time'],data['ValStr'],'\n'))
                historyData.append((data['PNO'],data['Time'],data['ValStr']))

        return historyData

if __name__ == '__main__':
    fo = open('in.txt' , 'r')
    line = fo.readline().strip()
    count = 1
    while line:
        '''print ('%s : %s' % (count , line.strip()))
        count += 1
        #print ("\n")'''
        analogNo = line
        print ('下载%s数据' % analogNo)
        tfo = open("timegap.txt" , "r")
        st = tfo.readline().strip()
        while st:
            startTime = str_to_datetime(st)
            endTime = str_to_datetime(tfo.readline().strip())
            granularity = tfo.readline().strip()
            m = Main(analogNo , startTime , endTime , granularity)
        #m.readinput()
            data = m.computing()
            mssql = sqlserver_connector.MSSQL(host = "localhost" , user = "sa" , pwd = "123456" , db = "EnergyMonitor")
            for d in data:
                sql = "insert into AnalogHistoryHour(AHH_AnalogNo , AHH_HTime , AHH_Value) values(%s , '%s' , %s)" % (d[0] , d[1] , d[2])
                mssql.ExecNonQuery(sql)
                #print ('%s %s %s' % (d[0] , d[1] , d[2]))
            st = tfo.readline().strip()
        tfo.close()
        print ('成功保存%s数据' % analogNo)
        line = fo.readline().strip()


    fo.close()
