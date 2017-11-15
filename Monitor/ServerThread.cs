
using System.Timers;
using WebSocketSharp.Server;
using NvApiWrapper;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Monitor
{
    struct GpuData
    {
        public NvClocks Clock;
        public NvGPUThermalSettings Sensors;
        public NvPStates PStates;

        public override string ToString()
        {

            var data = new {

                tag = "performance",
                performances = new  {
                    Clocks = new { coreClock = Clock.CoreClock, memoryClock = Clock.MemoryClock },
                    Sensors = new { currentTemp = Sensors.CurrentTemp },
                    PStates = new { gpuUsage = PStates.GpuUsage, gpuPower = PStates.GpuPower }
                }
            };

       
            return JsonConvert.SerializeObject(data);
        }
    }

    class ServerThread 
    {

        WebSocketServer wssv;
        NvPhysicalGpuHandle[] gpuHandles = new NvPhysicalGpuHandle[64];
        int count;
        public void Run()
        {
            bool IsAvailable = NVAPI.IsAvailable;

           
            wssv = new WebSocketServer(8080);
            wssv.AddWebSocketService<Stat>("/Stats");
            wssv.Start();
            if (!IsAvailable)
            {
                throw new Exception("Nvidia driver is not happy");
            }

            NvStatus status = NVAPI.NvAPI_EnumPhysicalGPUs(gpuHandles, out count);
            Timer myTimer = new Timer();
            myTimer.Elapsed += new ElapsedEventHandler(sendDate);
            myTimer.Interval = 500;
            myTimer.Enabled = true;
        }


        private void sendDate(object source, ElapsedEventArgs e)
        {
            var clock = getGpuClocks();
            var sensors = getSensors();
            var states = getStates();

            GpuData gpuData = new GpuData
            {
                Clock = clock,
                Sensors = sensors,
                PStates = states
            };


            wssv.WebSocketServices["/Stats"].Sessions.Broadcast(gpuData.ToString());
        }

      

        private NvClocks getGpuClocks()
        {
            var clock = new NvClocks
            {
                Version = NVAPI.GPU_CLOCKS_VER,
                ClockType = 0
            };
            NvStatus status = NVAPI.NvAPI_GPU_GetAllClocks(gpuHandles[0], ref clock);
            return clock;
        }

        private NvGPUThermalSettings getSensors()
        {
            var sensors = new NvGPUThermalSettings
            {
                Version = NVAPI.GPU_THERMAL_SETTINGS_VER,
                Count = 3
            };
            NvStatus status = NVAPI.NvAPI_GPU_GetThermalSettings(gpuHandles[0], 15, ref sensors);

            return sensors;
        }

        private NvPStates getStates()
        {
            var states = new NvPStates
            {
                Version = NVAPI.GPU_PSTATES_VER
            };

            NvStatus status = NVAPI.NvAPI_GPU_GetPStates(gpuHandles[0], ref states);


            return states;
        }

    }
}

