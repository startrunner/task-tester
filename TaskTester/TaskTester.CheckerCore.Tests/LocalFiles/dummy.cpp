//note: Compile with GNU G++ Compiler
#include <iostream>
#include <windows.h>
#include <cstdio>
using namespace std;

char* randomChars="dhwdhgwdhgdwhjjhdwhuwundwndwudwudwuahdwuhdwdwgdwgdwhodwhdwgguwdihoiw5445544545dhwdugiwdgywdguiwdhw";
int main()
{
    //SetErrorMode(3);
    string command;
    for(;;)
    {
        cin>>command;
        if(command=="crash")
        {
            int i=5/0;
        }
        else if(command=="sum")
        {
            int a, b;
            cin>>a>>b;
            printf("%d\n", a+b);
        }
        else if(command=="wait")
        {
            int t;
            cin>>t;
            Sleep(t);
        }
        else if(command=="end")
        {
            return 0;
        }
        else if(command=="exit")
        {
            int code;
            cin>>code;
            return code;
        }
        else if(command=="continuity")
        {
            int count;
            cin>>count;
            for(int i=0;i<count;i++)
            {
                printf("%d %s\n", i, randomChars);
            }
        }
    }
}
