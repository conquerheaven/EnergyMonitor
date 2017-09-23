import requests

class DownloadData(object):

    __url = 'http://localhost:15440/Admin/Information/QueryAllEnergyHistoryAjax'

    def __init__(self , analogNo , startTime , endTime , granularity):
        self.analogNo = analogNo
        self.startTime = startTime
        self.endTime = endTime
        self.granularity = granularity

    def fetchData(self):
        params = {
            'analogNo':self.analogNo,
            'startTime':self.startTime,
            'endTime':self.endTime,
            'granularity':self.granularity
            }
        headers = {
            'X-Requested-With':'XMLHttpRequest'
            }
        response = requests.get(DownloadData.__url,params = params , headers = headers)
        if response.status_code == 200: return True , response.json()
        else: return False , response.text
