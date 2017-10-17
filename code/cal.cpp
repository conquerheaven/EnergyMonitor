#include <iostream>
#include <cstdio>
#include <string>
#include <cmath>
#include <vector>
using namespace std;

struct point{
    double x , y;
    point(double x , double y):x(x),y(y){}
};

vector<point> pv , allp;
vector<point> result;

double L(double x){
    double sum = 0;
    for(int i = 0; i < pv.size(); i++){
        double li = 1.0;
        for(int j = 0; j < pv.size(); j++){
            if(i == j) continue;
            li = li*(x - pv[j].x)/(pv[i].x - pv[j].x);
        }
        sum += pv[i].y * li;
    }
    return sum;
}

void computing(){
    for(int i = 502; i <= 527; i++){
        result.push_back(point(i , L(i)));
    }
    int cnt = 0;
    for(int i = 0; i < allp.size(); i++){
        cout << allp[i].x  << " " << allp[i].y << endl;
        if(i != 0){
            for(int j = allp[i].x+1; j < allp[i-1].x; j++){
                cout << j << " " << 0 << endl;
            }
        }
    }
    for(int i = 0; i < result.size(); i++){
        cout << result[i].x << " " << result[i].y << endl;
    }
}

int main(){
    freopen("in.txt" , "r" , stdin);
    freopen("out.txt" , "w" , stdout);
    double x , y;
    int C = 0;
    while(cin >> x >> y){
        if(C == 0) pv.push_back(point(x , y));
        C = (C+1)%20;
        allp.push_back(point(x, y));
    }
    computing();
    return 0;
}
