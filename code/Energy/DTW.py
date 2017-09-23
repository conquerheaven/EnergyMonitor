import math
class DTW:
    def __init__(self):
        self.list1 = []
        self.list2 = []
        self.dis = []

    def __getDis(self):
        #print (self.list1)
        #print (self.list2)
        self.dis = []
        for i in range(len(self.list1)):
            tdis = []
            for j in range(len(self.list2)):
                tdis.append(math.fabs(self.list2[j]-self.list1[i]))
            self.dis.append(tdis)

    def similarity(self , list1 , list2):
        self.list1 = list1
        self.list2 = list2
        self.__getDis()
        dp = []
        dx = [0 , 1 , 1]
        dy = [1 , 0 , 1]
        for i in range(len(self.list1)):
            tdp = []
            for j in range(len(self.list2)):
                tdp.append(100000)
            dp.append(tdp)
        dp[0][0] = self.dis[0][0]
        for i in range(len(dp)):
            for j in range(len(dp[i])):
                for k in range(3):
                    x = i+dx[k]
                    y = j+dy[k]
                    if x < len(list1) and y < len(list2):
                        dp[x][y] = min(dp[x][y],dp[i][j]+self.dis[x][y])
        return dp[len(list1)-1][len(list2)-1]/(len(list1)+len(list2))


if __name__ == '__main__':
    dtw = DTW()
    list1 = [1 , 1 , 2 , 4]
    list2 = [2 , 1 , 3 , 1]
    print (dtw.similarity(list1 , list2))
    list1 = [1 , 1 , 1 , 1]
    list2 = [2 , 2 , 2 , 2]
    print (dtw.similarity(list1 , list2))
