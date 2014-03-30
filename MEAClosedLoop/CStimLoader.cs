using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MEAClosedLoop
{
    class CStimLoader
    {
        private XmlTextReader reader;
        public List<ushort[]> pdataPatterns;
        public List<UInt64[]> tdataPatterns;
        public List<ushort[]> psyncPatterns;
        public List<UInt64[]> tsyncPatterns;
        public string FileStimConfigPath;

        public CStimLoader(string ConfigPath = "../../Config.xml")
        {
            pdataPatterns = new List<ushort[]>();
            tdataPatterns = new List<ulong[]>();
            tsyncPatterns = new List<ulong[]>();
            psyncPatterns = new List<ushort[]>();
            FileStimConfigPath = ConfigPath;
            LoadConfig();
        }
        public bool LoadConfig()
        {
            XmlTextReader reader = new XmlTextReader(FileStimConfigPath);
            while (reader.Read())
            {
                int id;
                switch (reader.NodeType)
                {
                    #region ParseSpace
                    case XmlNodeType.Element:
                        //select One pattern
                        if (reader.Name.Equals("pattern"))
                        {
                            int.TryParse(reader.GetAttribute("id"), out id);
                            string patterntype = reader.GetAttribute("type");
                            List<ushort> currentpdata = new List<ushort>();
                            List<UInt64> currenttdata = new List<UInt64>();
                            //load stims pattern
                            reader.Read();
                            while (reader.Read() && reader.Name.Equals("stim"))
                            {
                                ulong valueTime;
                                ushort valueData;
                                ushort.TryParse(reader.GetAttribute("voltage"), out valueData);
                                currentpdata.Add(valueData);
                                ulong.TryParse(reader.GetAttribute("time"), out valueTime);
                                currenttdata.Add(valueTime);
                                reader.Read();
                            }
                            if (patterntype.Equals("stim"))
                            {
                                pdataPatterns.Add(currentpdata.ToArray());
                                tdataPatterns.Add(currenttdata.ToArray());
                            }
                            if (patterntype.Equals("sync"))
                            {
                                psyncPatterns.Add(currentpdata.ToArray());
                                tsyncPatterns.Add(currenttdata.ToArray());
                            }
                            //end loading pattern
                        }
                        break;
                    #endregion 
                }

            }
            //Configpath parsed
            return true;
        }
    }
}
