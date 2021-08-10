using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmsville_Dial
{
    class ProcessSpaceNavData
    {
        public static ProcessSpaceNavData ProcessSpaceNavDataOBJ = new ProcessSpaceNavData();
        public void process(string activeapp, string[] gyrodata, string[] planedata)
        {
            
            if (activeapp.Contains("SOLIDWORKS"))
            {
                double[] Gdata = { 0, 0, 0 };
                double[] Pdata = { 0, 0, 0 };
                try
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Gdata[i] = double.Parse(gyrodata[i]) / 57.296;
                        Pdata[i] = double.Parse(planedata[i]);
                    }
                    //System.Console.WriteLine("planar =  " + Gdata[0] + "   " + Gdata[1] + "   " + Gdata[2]);
                    InApp_Operations.SOLIDWORKS.swObj.setRawValues(Gdata, Pdata);
                }
                catch (Exception)
                {

                    
                }

                
            }
            else if (activeapp.Contains("Fusion 360"))
            {
                
                double[] Gdata = { 0, 0, 0 };
                double[] Pdata = { 0, 0, 0 };
                try
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Gdata[i] = double.Parse(gyrodata[i])/57.296;
                        Pdata[i] = double.Parse(planedata[i]);
                    }
                    InApp_Operations.FUSION360.f360Obj.setRawValues(Gdata, Pdata);
                }
                catch (Exception)
                {


                }


            }
        }
    }
}
