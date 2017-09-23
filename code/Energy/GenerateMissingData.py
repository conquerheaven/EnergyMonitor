import datetime
import sqlserver_connector

def str_to_datetime(s):
    return datetime.datetime.strptime(s, '%Y-%m-%d %H:%M:%S')

def isBetween2389(t):
    if t >= str_to_datetime('2016-02-01 00:00:00') and t < str_to_datetime('2016-04-01 00:00:00'):
        return True
    elif t >= str_to_datetime('2016-08-01 00:00:00') and t < str_to_datetime('2016-10-01 00:00:00'):
        return True
    else:
        return False

if __name__ == '__main__':
    AHH_AnalogNoList = [13162 , 13475 , 200010]
    mssql = sqlserver_connector.MSSQL(host = "localhost" , user = "sa" , pwd = "123456" , db = "EnergyMonitor")
    AHHList = []
    fo = open('missingGap' , "w")
    for AnalogNo in AHH_AnalogNoList:
        sql = 'select * from AnalogHistoryHour where AHH_AnalogNo = %s and AHH_HTime between \'%s\' and \'%s\'' % (AnalogNo , '2016-02-01 00:00:00' , '2016-12-01 00:00:00')
        print (sql)
        AHHList = mssql.ExecQuery(sql)
        l = len(AHHList)
        i = 0
        while i < l:
            #print (i)
            #print (AHHList[i])
            AHH = AHHList[i]
            count = 0
            startTime = AHH[1]
            endTime = AHH[1]
            while isBetween2389(AHH[1]) and AHH[2] == 0 and i < l:
                count += 1
                endTime = AHH[1]
                i += 1
                AHH = AHHList[i]
            if count >= 24 and count <= 72:
                fo.writelines('%s %s %s %s\n' % (AnalogNo , startTime , endTime , count))
            i += 1
    fo.close()