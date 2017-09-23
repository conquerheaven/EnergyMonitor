import math
class Interpolate:


    def __initial(self):
        self.length = len(self.lineList)
        self.dp = []        #dp[i][j]表示第i-n个点，总和为j时，能够取得的最近相似度
        self.next = []       #next[i][j]表示第i-n个点，总和为j，取得最近相似度时，第i个点的取值
        self.__maxn = 1010  #j的最大值
        self.__MaxValue = 0 #next[i][j]的最大值
        self.__step = 0     #枚举next[i][j]时的步长
        self.resultLine = []#结果曲线
        for i in range(self.length):
            tdp = []
            for j in range(self.__maxn):
                tdp.append(-1)
            self.dp.append(tdp)
            self.next.append(tdp)
        for v in self.lineList:
            self.__MaxValue = max(self.__MaxValue , v)
        self.__step = self.__MaxValue/100

    def __DP(self , k , v):
        if k >= self.length: return 0
        if self.dp[k][v] != -1: return self.dp[k][v]
        ans = self.length*self.__maxn
        if k == self.length-1:
            ans = self.__getDistance(k , v)
            self.next[k][v] = v
        else:
            nxt = self.__step
            while nxt <= self.__MaxValue:
                tans = self.__getDistance(k , nxt) + self.DP(k+1 , v-nxt)
                if tans < ans:
                    ans = tans
                    self.next[k][v] = nxt
                nxt += self.__step
        self.dp[k][v] = ans
        return self.dp[k][v]

    def __getDistance(self,k,v):
        dif = 0
        for i in range(self.lineList):
            if self.lineList[i][k] == 0: continue
            dif += math.fabs(self.lineList[i][k] - v)
        return dif



    def interpolate(self,lineList):
        k = len(lineList)
        n = len(lineList[0])
        sumXik = []
        sumXnk = 0.0
        for i in range(n):
            sum = 0.0
            for j in range(k):
                sum += lineList[j][i]
            sumXik.append(sum)
            sumXnk += sum
        lamda = (2*sumXnk-200*k)/n
        x = []
        for i in range(n):
            x.append((-1*lamda/2+sumXik[i])/k)
        return x
