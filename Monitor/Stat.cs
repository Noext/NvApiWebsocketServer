
using WebSocketSharp;
using WebSocketSharp.Server;
using NvApiWrapper;
using System.Timers;
using System;

namespace Monitor
{
    public class Stat : WebSocketBehavior
    {
       



        protected override void OnMessage(MessageEventArgs e)
        {

        }

        protected override void OnOpen()
        {
            base.OnOpen();
            
         
        }


        public Stat()
        {
           
           
           

        }

        

      

    }
}

