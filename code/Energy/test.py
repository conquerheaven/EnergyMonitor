
class a():
    n = 0

    def add(self,x):
        self.n += x
    def printn(self):
        print(self.n)

if __name__ == '__main__':
    ob1 = a()
    ob2 = a()

    ob1.add(1)
    ob1.printn()
    ob2.printn()
'''
class task_queue:
    queue=[]

    def append(self,obj):
        self.queue.append(obj)

    def print_queue(self):
        print (self.queue)


if __name__=="__main__":
    a=task_queue()
    b=task_queue()
    c=task_queue()

    a.queue.append('tc_1')

    print(a.queue , b.queue , c.queue)

    a.print_queue()
    b.print_queue()

'''