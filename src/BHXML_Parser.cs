//	(C) Copyright Ian Keough 2009
//	This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Windows.Forms;

using BuroHappold.Analysis;
using BuroHappold;
using BuroHappold.Analysis.Elements;

namespace BuroHappold.Interface
{
	/// <summary>
	/// Description of BHXML_Parser.
	/// </summary>
	public class BHXml_Parser
	{	
		#region private members
		string _docPath;				//Xml document path		
		
		Hashtable _nodeHash = new Hashtable();				//hash table of all nodes;
		Hashtable _barHash = new Hashtable();				//hash table of all bars;
		Hashtable _plateHash = new Hashtable();			//hash table of all plates;
		
		FileStream _fs;
		
		XmlDocument _xml;
		#endregion
		
		#region properties
		/// <summary>
		/// The Xml document path.
		/// </summary>
		public string XmlPath
		{
			get
			{
				return _docPath;
			}
		}
		
		public XmlDocument XmlDoc
		{
			get
			{
				return _xml;
			}
		}
		#endregion
		
		#region methods
		/// <summary>
		/// Constructor
		/// </summary>
		public BHXml_Parser()
		{
			
		}
		
		/// <summary>
		/// Load an xml document
		/// </summary>
		public void LoadXml(string path)
		{
			//load the xml document
			_fs = new FileStream(path, FileMode.Open, 
			                               FileAccess.Read,
			                               FileShare.ReadWrite);
			_xml = new XmlDocument();
			_xml.Load(_fs);
	
			_docPath = path;	//set the private member
		}
		
		/// <summary>
		/// Read element data from the XML file.
		/// </summary>
		/// <param name="_doc">The XML document.</param>
		/// <param name="_nodeInfo">A List of node objects.</param>
		/// <param name="_barInfo">A List of bar objects</param>
		public void ReadFromXML(XmlDocument _doc, ref List<BHNode> _nodeInfo, ref List<BHBar> _barInfo) //, ref List<BHPlate> _plateInfo)
		{
			try
			{
			#region nodeInfo
			
			//get the points element
			XmlNodeList points = _doc.GetElementsByTagName("nodes");	//the parent of all nodes
			
	   		//get the list of point nodes
			XmlNodeList nodes = points[0].ChildNodes;
			
			//for every point node
			//add to the array list in this order:
			//uniqueId, x, y, z
			
			for(int i=0; i<nodes.Count; i++)
   			{
				//get the attributes on the node
				//this should be the 'catiaid' and the 'robotid'
				XmlAttributeCollection xmlAttribs = nodes[i].Attributes;
				
				if (xmlAttribs == null)
				{
					Console.WriteLine("There are no attributes on the node.");
					return;
				}
				

				int uniqueId 	= 	System.Convert.ToInt16(xmlAttribs[0].Value);
				
				//get the child nodes by name
				//[0]x
				//[1]y
				//[2]z
				
				XmlNodeList children = nodes[i].ChildNodes;
				
				//get the positions
				double posX = System.Convert.ToDouble(children[0].InnerText);	//x component
				double posY = System.Convert.ToDouble(children[1].InnerText);	//y component
				double posZ = System.Convert.ToDouble(children[2].InnerText);	//z component
				
				//create the node object
				BHNode n = new BHNode(uniqueId, posX, posY, posZ);
				_nodeInfo.Add(n);
				
				//add the node object to the hashtable
				_nodeHash.Add(n.Id, n);
				
			}
			#endregion nodeInfo
			
			#region barInfo
			
			XmlNodeList lines = _doc.GetElementsByTagName("bars");	//the parent of all bars
			
			//get the list of point nodes
			XmlNodeList bars = lines[0].ChildNodes;
			
			//for every bar
			//add to the array list in this order:
			//catiaStart, catiaEnd, robotStart, robotEnd, stress_axial
			
			for(int i=0;i<bars.Count;i++)
   			{
				//get the attributes on the node
				//this should be the uniqueId
				XmlAttributeCollection xmlAttribs = bars[i].Attributes;
				
				int uniqueId 	= System.Convert.ToInt16(xmlAttribs[0].Value);

				//get the child nodes by name
				//[0]start node, [1]end node, [2]ea, [3]force control, [4]cable property, 
				//[4]slack length , [5] color

				//get the child nodes by name - this should be the 'position'
				XmlNodeList children = bars[i].ChildNodes;
				int startNode 		= System.Convert.ToInt16(children[0].InnerText);		//the start node
				int endNode			= System.Convert.ToInt16(children[1].InnerText);		//the end node
				double ea			= System.Convert.ToDouble(children[2].InnerText);		//the ea
				int forceControl	= System.Convert.ToInt16(children[3].InnerText);		//the force control
				int cprop			= System.Convert.ToInt16(children[4].InnerText);		//the cable prop.
				double slackLength 	= System.Convert.ToDouble(children[5].InnerText);		//slack length
				float red			= (float)System.Convert.ToDouble(children[6].Attributes[0].Value);	//red
				float green			= (float)System.Convert.ToDouble(children[6].Attributes[1].Value);	//green
				float blue			= (float)System.Convert.ToDouble(children[6].Attributes[2].Value);	//blue

				//find the nodes that correspond to the start and end nodes

				BHNode start 	= _nodeHash[startNode] as BHNode;
				BHNode end 		= _nodeHash[endNode] as BHNode;
				
				BHBar b = new BHBar(uniqueId, start,
				                    end);
				_barInfo.Add(b);
			
				//add the bar to the hashtable
				_barHash.Add(b.Id, b);
				
			}
			
			#endregion barInfo
		
			#region plateInfo
			
//			//get the plates in the Xml
//			XmlNodeList shapes = _doc.GetElementsByTagName("plates");
//			
//			//get the list of plate nodes
//			XmlNodeList plates = shapes[0].ChildNodes;
//			
//			//iterate through all plates adding 
//			for(int i=0;i<plates.Count;i++)
//   			{
//				XmlNode plate = plates[i] as XmlNode;
//				
//				List<string> catiaPoints = new List<string>();	//to hold the catia points
//				List<int> robotPoints = new List<int>();		//to hold the robot points
//				
//				//get the attributes on the plate
//				//this should be the uniqueId
//				XmlAttributeCollection xmlAttribs = plate.Attributes;
//				
//				int uniqueId 	= System.Convert.ToInt16(xmlAttribs[0].Value);
//				
//				//get the children of catiaNodes
//				XmlNode platePointsFamily = plate.ChildNodes[0];
//				XmlNodeList platePoints = platePointsFamily.ChildNodes;
//				
//				foreach (XmlNode platePoint in platePoints)
//				{
//					XmlAttributeCollection plateAttribs = platePoint.Attributes;
//					catiaPoints.Add(System.Convert.ToString(plateAttribs[0].Value));
//					robotPoints.Add(System.Convert.ToInt16(plateAttribs[1].Value));
//				}
//
//				
//				//TODO: add full plate support
//				
//				//create the plate object
//				BHPlate p = new BHPlate(uniqueId);
//				_plateInfo.Add(p);
//				
//				//add the plate to the hash table
//				_plateHash.Add(p.Id, p);
//				
//			}
			#endregion
			
			}
			catch(Exception e)
			{
				MessageBox.Show(e.StackTrace);
			}
			
			_fs.Close();	//close the file stream
		}
		
		
		/// <summary>
		/// Read Net Builder data from an XML file 
		/// </summary>
		/// <param name="_doc">the XML document</param>
		/// <param name="_zones">List: each item is the number of zones for a net</param>
		/// <param name="_sideLengths">List: each item is length of a side for a net</param>
		/// <param name="_topAngles">List: </param>
		/// <param name="_colorFilepaths">List: each element is a filepath to a file that determines a net's color</param>
		/// <param name="_subNets">List: each element is an ArrayList of subnets that compose a net</param>
		public void ReadNetBuilderInputs(XmlDocument _doc,
		                                 ref List<int> _zones,
		                                 ref List<double> _sideLengths,
		                                 ref List<double> _topAngles,
		                                 ref List<string> _colorFilepaths,
		                                 ref List<ArrayList> _subNets)
		{
			try
			{
				#region nets
				// get the nets XML element
				XmlNodeList netsElement = _doc.GetElementsByTagName("nets");	//the parent of all nets
			
	   			// get the nets
				XmlNodeList netsChildren = netsElement[0].ChildNodes;
			
				#region netInfo
				ArrayList subnetList;	// used to hold subnet information
				
				for(int i=0; i<netsChildren.Count; i++)
				{
					// get net data
					XmlNodeList netData = netsChildren[i].ChildNodes;
					
					// add net data to appropriate List (passed in as parameter)
					_zones.Add(System.Convert.ToInt16(netData[0].InnerText));
					_sideLengths.Add(System.Convert.ToDouble(netData[1].InnerText));
					_topAngles.Add(System.Convert.ToDouble(netData[2].InnerText));
					_colorFilepaths.Add(System.Convert.ToString(netData[3].InnerText));
					
					// get subnets from document
					XmlNodeList subnetChildren = netData[4].ChildNodes;
					
					// create container
					subnetList = new ArrayList();
					
					// populate container with data
					for(int j=0; j<subnetChildren.Count; j++)
					{
						subnetList.Add(System.Convert.ToDouble(subnetChildren[j].InnerText));
					}
					
					// add container to List
					_subNets.Add(subnetList);
					
				}
				#endregion netInfo
			
				#endregion nets
			
			}
			catch(Exception e)
			{
				MessageBox.Show(e.StackTrace);
			}
		}
			
			
		/// <summary>
		/// Save new element data to the XML file.
		/// </summary>
		/// <param name="_doc">The XML document</param>
		/// <param name="_nodeInfo">A List of node objects.</param>
		/// <param name="_barInfo">A List of bar objects.</param>
		public void UpdateXML(ref XmlDocument _doc, ref List<BHNode> _nodeInfo, ref List<BHBar> _barInfo) //, ref List<BHPlate> _plateInfo)
		{
			//get the lists of nodes and bars
			XmlNodeList nodes 	= _doc.GetElementsByTagName("node");
			XmlNodeList bars 	= _doc.GetElementsByTagName("bar");
			XmlNodeList plates	= _doc.GetElementsByTagName("plate");

			#region update nodes
			for(int i=0;i<nodes.Count;i++)
   			{
				BHNode n = (BHNode)_nodeInfo[i];	//the matching Node in the List
				
				//get the attributes on the node
				//this should be the 'catiaid' and the 'robotid'
				XmlAttributeCollection xmlAttribs = nodes[i].Attributes;
				
				//update node Ids
				xmlAttribs[0].Value = n.Id.ToString();

				//get the child nodes by name
				//[0]position
				//[1]displacement
				XmlNodeList children = nodes[i].ChildNodes;
				
				//update the positions
				children[1].InnerText = n.X.ToString();	
				children[2].InnerText = n.Y.ToString();
				children[3].InnerText = n.Z.ToString();

			}
			#endregion update nodes
			
			#region update bars
			
			for(int i=0;i<bars.Count;i++)
   			{
				BHBar b = (BHBar)_barInfo[i];	//the matching bar in the list
				
				//get the attributes on the node
				//this should be the 'catiaid' and the 'robotid'
				XmlAttributeCollection xmlAttribs = bars[i].Attributes;

				xmlAttribs[0].Value = b.Id.ToString();
				
				//get the child nodes by name
				//[0]startpoint catiaid, [1]endpoint catiaid, [3]startpoint robotid, [4]endpont robotid, [5]stress stressAxial

				XmlNodeList children = bars[i].ChildNodes;

				children[1].InnerText = b.StartNode.ToString();
				children[2].InnerText = b.EndNode.ToString();
				children[3].InnerText = b.EA.ToString();
				children[4].InnerText = b.Control.ToString();
				children[5].InnerText = b.CPropId.ToString();
				children[6].InnerText = b.SlackLength.ToString();
				children[7].Attributes[0].Value = b.Color.R.ToString();
				children[7].Attributes[1].Value = b.Color.G.ToString();
				children[7].Attributes[2].Value = b.Color.B.ToString();
				

			}

			#endregion update bars

			#region update plates
//			for (int i=0; i<plates.Count;i++)
//			{
//				BHPlate p = (BHPlate)_plateInfo[i];	//the matching plate in the list
//				
//				//get the attributes on the node
//				//this should be the uniqueId
//				XmlAttributeCollection xmlAttribs = plates[i].Attributes;
//				
//				xmlAttribs[0].Value = p.Id.ToString();
//
//				XmlNode platePointsFamily = plates[i].ChildNodes[0];
//				XmlNodeList platePoints = platePointsFamily.ChildNodes;
//				
//				//update the robot nodes
////				for (int j=0; j<platePoints.Count; j++)
////				{
////					XmlAttributeCollection platePointAttribs = platePoints[j].Attributes;
////					platePointAttribs[0].Value = p.CatiaPoints[j].ToString();
////					platePointAttribs[1].Value = p.RobotNodes[j].ToString();
////				}
//				 
//			}
			#endregion
			_doc.Save(this._docPath);	//save the changes to the xml
		}
		
		public void Reset()
		{
			//reset the 
			_nodeHash.Clear();
			_barHash.Clear();
			_plateHash.Clear();
		}
		#endregion
		
	}
}
