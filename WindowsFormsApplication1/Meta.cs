using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;
using System.Diagnostics;
using System.IO;

namespace WindowsFormsApplication1
{
    class Meta
    {
        XmlDocument doc = new XmlDocument();
        DataSet ds = new DataSet();
        public Dictionary<string, List<string>> metaDict = new Dictionary<string, List<string>>();
        public MetaFileNode fileNode = null;
        string fullFileName;

        public string findFullFileName(string filename)
        {

            return fullFileName;
        }
        //override constructor, init from XML file path
        public Meta(string path)
        {
            Debug.WriteLine("path ~~~~~~~~~~~~~~~~~~~~" + path);
            ds.ReadXml(path);
            doc.Load(path);
            loadMeta();
        }

        private void loadMeta(){
            List<string> metadatavalue_list = new List<string>();
            XmlNodeList metadataTypeNodes = doc.SelectNodes("/metadatatable/metadatatype");

             foreach (XmlNode metadataTypeNode in metadataTypeNodes) //top level  
             {
                 string metadataType = metadataTypeNode.Attributes.GetNamedItem("value").Value;
                 Debug.WriteLine(metadataType);

                 XmlNodeList metadataValueNodes = metadataTypeNode.SelectNodes("metadatavalue");

                 foreach (XmlNode metadataValueNode in metadataValueNodes)   // 2nd level
                 {
                     string metadataValue = metadataValueNode.Attributes.GetNamedItem("value").Value;
                   
                     metadatavalue_list.Add(metadataValue);
                 }
                 metaDict.Add(metadataType, metadatavalue_list);
                 metadatavalue_list = new List<string>();
             }
        }

        public Dictionary<string, List<string>> lookup(string filename, bool isfullpath) {
            if (isfullpath) return lookup(filename);


            filename = filename.Trim();
            XmlNodeList metadataFileNodes = doc.SelectNodes("/metadatatable/metadatatype/metadatavalue/file");
     

            foreach (XmlNode metadataFileNode in metadataFileNodes)
            {
                //check if the file exsist
                string full_path = metadataFileNode.InnerText;
                string file_name = full_path.Split(Path.DirectorySeparatorChar).Last();

                if (file_name.Equals(filename))
                {
                    fullFileName = full_path;
                    Debug.WriteLine("found file ~~~~~~~~~~~~~~~~~ " + filename);
                    fileNode = new MetaFileNode(filename);
                    break;
                }
            }

            //if exsist
            if (fileNode != null)
            {
                foreach (XmlNode metadataFileNode in metadataFileNodes)
                {
                    string full_path = metadataFileNode.InnerText;
                    string file_name = full_path.Split(Path.DirectorySeparatorChar).Last();

                    if (file_name.Equals(filename))
                    {
                        string key1 = metadataFileNode.ParentNode.ParentNode.Attributes.GetNamedItem("value").Value;
                        string val = metadataFileNode.ParentNode.Attributes.GetNamedItem("value").Value;
                        fileNode.SetDictObject(key1, val);

                    }
                }
                printDict(fileNode.fileMetaDict);
                return fileNode.fileMetaDict;
            } //end  if (fileNode != null), fileNode built successfully


            return null;

        }

        public Dictionary<string, List<string>> lookup(string filename)
        {

            filename = filename.Trim();
            XmlNodeList metadataFileNodes = doc.SelectNodes("/metadatatable/metadatatype/metadatavalue/file");
            
            foreach (XmlNode metadataFileNode in metadataFileNodes)
            {
                //check if the file exsist
                if (filename.Equals(metadataFileNode.InnerText))
                {
                    Debug.WriteLine("found file " + filename);
                    fileNode = new MetaFileNode(filename);
                    break;
                }
            }

            //if exsist
            if (fileNode != null)
            {
                foreach (XmlNode metadataFileNode in metadataFileNodes)
                {
                    if (filename.Equals(metadataFileNode.InnerText))
                    {
                        
                      string key1 = metadataFileNode.ParentNode.ParentNode.Attributes.GetNamedItem("value").Value;
                      string val = metadataFileNode.ParentNode.Attributes.GetNamedItem("value").Value;
                      fileNode.SetDictObject(key1, val);
                    
                    }
                }
                printDict(fileNode.fileMetaDict);
                return fileNode.fileMetaDict;
            } //end  if (fileNode != null), fileNode built successfully


            return null;
        }


        public void addNode(string level1, string level2, string filename)
        {
            XmlNode newNode = doc.CreateElement("file");
            newNode.InnerText = filename;

            //location to append
            XmlNodeList metadatatypeNodes = doc.SelectNodes("/metadatatable/metadatatype");
            foreach (XmlNode metadatatypeNode in metadatatypeNodes)
            {
                if (level1.Equals(metadatatypeNode.Attributes.GetNamedItem("value").Value)){
                    XmlNodeList metadatavalueNodes = metadatatypeNode.ChildNodes;
                    foreach (XmlNode metadatavalueNode in metadatavalueNodes) {
                        if (level2.Equals(metadatavalueNode.Attributes.GetNamedItem("value").Value))
                        {
                            metadatavalueNode.AppendChild(newNode);
                            Debug.WriteLine("successfully append newnode");
                            break;
                        }
                    }

                    break;
                }
            }
        }

        public void deleteNode(string level1, string level2, string filename) {

            XmlNodeList metadatatypeNodes = doc.SelectNodes("/metadatatable/metadatatype");
            foreach (XmlNode metadatatypeNode in metadatatypeNodes)
            {
                if (level1.Equals(metadatatypeNode.Attributes.GetNamedItem("value").Value))
                {
                    XmlNodeList metadatavalueNodes = metadatatypeNode.ChildNodes;
                    foreach (XmlNode metadatavalueNode in metadatavalueNodes)
                    {
                        if (level2.Equals(metadatavalueNode.Attributes.GetNamedItem("value").Value))
                        {
                            XmlNodeList metadatafileNodes = metadatavalueNode.ChildNodes;
                            for (int i = metadatafileNodes.Count - 1; i >= 0; i--)
                            {
                                if (filename.Equals(metadatafileNodes[i].InnerText))
                                {
                                    metadatafileNodes[i].ParentNode.RemoveChild(metadatafileNodes[i]);
                                    Debug.WriteLine("successfully remove node");
                                    break;
                                }     
                            }
                       
                            break;
                        }
                    }

                    break;
                }
            }
        
        }

        public void save(string filepath) {

            doc.Save(filepath);
        }

        public void printDict(Dictionary<string, List<string>> dict)
        {
            Debug.WriteLine("============== dict ============= ");
            foreach (string key in dict.Keys)
            {
                Debug.WriteLine("==============key = " + key);
                foreach (string val in dict[key])
                {
                    Debug.WriteLine(val);
                }
                Debug.WriteLine("");
            }
        }
    }

    public class MetaFileNode
    {
        string filename;
        public Dictionary<string, List<string>> fileMetaDict;

        public MetaFileNode(string name){
            filename = name;
            fileMetaDict = new Dictionary<string,List<string>>();
        }

        public void SetDictObject(string key1, string val){
            if (fileMetaDict.ContainsKey(key1))
            {
                if (fileMetaDict[key1].Contains(val)) return;
                fileMetaDict[key1].Add(val);
            }
            else
            {
                List<string> list = new List<string>();
                list.Add(val);
                fileMetaDict.Add(key1, list);
            }

        }

        public void removeDictObject(string key1, string val) {
            if (fileMetaDict.ContainsKey(key1))
            {
                if (fileMetaDict[key1].Contains(val)) { fileMetaDict[key1].Remove(val); };
                if (fileMetaDict[key1].Count == 0) { fileMetaDict.Remove(key1); };
            }
        
        }


    }
}
