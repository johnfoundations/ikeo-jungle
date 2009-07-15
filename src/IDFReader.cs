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
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

namespace ikeo
{
	
	/// <summary>
	/// Description of OBJReader.
	/// </summary>
	public class IDFReader
	{
		public List<Vector> verts;
		public List<Vector> textCoords;
		public List<Vector> norms;
		public List<Mesh> meshes;
		public ArrayList indices;
		public ArrayList textIndices;
		public ArrayList normIndices;
		public List<DataObject> dataObjects;
		
		public IDFReader(string fileName)
		{
			LoadMeshesFromIDF(fileName);
			LoadDataObjectsFromIDF(fileName);
		}
		
		public void LoadMeshesFromIDF(string fileName)
		{
			meshes = new List<Mesh>();	//a list to hold the meshes
			int meshCount = 0;
			
			int vertCounter 		= 0;
			verts 		= new List<Vector>();	//one big array to hold all the verts
			textCoords 	= new List<Vector>();	//another to hold the texture coords
			norms		= new List<Vector>();	//and one more to hold the normals
			
			indices	= new ArrayList();
			List<int> innerIndexList = new List<int>();		//list to hold inner face, texture, and normal indices
															//right now, it just holds face indices!!!
			textIndices = new ArrayList();
			List<int> innerTextIndices = new List<int>();
			normIndices = new ArrayList();
			List<int> innerNormIndices = new List<int>();
			
			#region read file
	        using(TextReader sr = new StreamReader(fileName))
	        {
	        	string line;
	        	while((line = sr.ReadLine()) != null)
	        	{
	        		string[] lineSplit = line.Split('#');
	        		
	        		//always add the first set of inner lists
	        		
	        		if(lineSplit[0] == "g")
	        		{	
	        			//REMOVED - SOME OBJ FILES DON'T USE 'g'
	        			if(meshCount == 0)
	        			{
							indices.Add(innerIndexList);		//add the first set of sub lists
							textIndices.Add(innerTextIndices);
							normIndices.Add(innerNormIndices);
	        			}
	        			else
	        			{
	        				indices.Add(innerIndexList);				//add the previous one
							textIndices.Add(innerTextIndices);
							normIndices.Add(innerNormIndices);
							
	        				innerIndexList = new List<int>();	//make a new one
	        				innerTextIndices = new List<int>();
	        				innerNormIndices = new List<int>();
	        				
	        				indices.Add(innerIndexList);		//add the one you've just created
							textIndices.Add(innerTextIndices);
							normIndices.Add(innerNormIndices);
	        			}
	        			
	        			meshCount++;	
	        		}
	        		
	        		if(lineSplit[0] == "v")
	        		{
	        			Vector v = new Vector(Convert.ToDouble(lineSplit[1]), 
	        			                      Convert.ToDouble(lineSplit[2]),
	        			                      Convert.ToDouble(lineSplit[3]));
	        			
	        			verts.Add(v);
	        			vertCounter++;	//this will keep track of how many verts we have
	        							//then you'll subtract this from 
	        			
	        		}
	        		else if(lineSplit[0] == "f")
	        		{
	        			//see if there are text coords and normals
	        			for(int i=1; i<lineSplit.Length; i++)
	        			{
		        			string[] innerSplit = lineSplit[i].Split('/');

		        			if(innerSplit.Length > 1)	// like : a/b/c
		        			{
		        				innerIndexList.Add(Convert.ToInt32(innerSplit[0]));
		        				
		        				if(innerSplit[1] != "")
		        				{
		        					innerTextIndices.Add(Convert.ToInt32(innerSplit[1]));
		        				}
		        				else{
		        					innerTextIndices.Add(0);	//add a zero as a placeholder
		        				}
		        				
		        				if(innerSplit.Length == 3)
		        				{
		        					if(innerSplit[2] != "")
		        					{
		        						innerNormIndices.Add(Convert.ToInt32(innerSplit[2]));
		        					}
		        					else{
		        						innerNormIndices.Add(0);
		        					}
		        				}
		        			}
		        			else	//just a
		        			{
		        				innerIndexList.Add(Convert.ToInt32(innerSplit[0]));
		        			}
	        			}

	        		}
	        		else if(lineSplit[0] == "vt")
	        		{
	        			Vector vt = new Vector(Convert.ToDouble(lineSplit[1]),
	        			                      Convert.ToDouble(lineSplit[2]),
	        			                      Convert.ToDouble(0.0));	//add a zero to fill the z of the Vector
	        			textCoords.Add(vt);
	        		}
	        	}
	        }
	        if(meshCount == 0)	//if it never hit a g tag - it's all one object
	        {
	        	indices.Add(innerIndexList);				//add the previous one
				textIndices.Add(innerTextIndices);
				normIndices.Add(innerNormIndices);
				meshCount++;
	        }
	        #endregion
	        
	        #region build meshes
	        for(int i=0; i<indices.Count; i++)	//loop through the entire arrayList of lists of indices
	        {

	        	List<int> ind = indices[i] as List<int>;	//the curr face index list 

	        	//stride by three creating new triangular meshes
	        	Mesh m = new Mesh();
	        	meshes.Add(m);
	        	
	        	//create the verts
	        	for(int j=0; j<verts.Count;j++)	//put all the verts in there
	        	{
	        		m.AddVertex(verts[j]);	//add the vertex by the face index
//	        		Debug.WriteLine("Adding vertex for " + verts[j].x +":" + verts[j].y + ":" + verts[j].z);
	        	}
	        	
	        	//create the texture coords - if they exist
	        	if(textCoords.Count != 0)
	        	{
		        	for(int j=0; j<m.vertexCount() ;j++)
		        	{
		        		m.vertices[j].tex = textCoords[j];
		        	}
	        	}
	        	
	        	//add the faces
	        	for(int j=0; j<ind.Count-2; j+=3)
	        	{
	        		
	        		m.AddTriangle(ind[j]-1, ind[j+1]-1, ind[j+2]-1);	//run through the indices by threes
//	        		Debug.WriteLine("Adding face for " + f[j] + ":" + f[j+1] + ":" + f[j+2]);
	        	}
	        	
	        	m.RemoveOrphanedVertices();	//get rid of everything not referenced
	        	
	        	//set the labels on the verts
	        	for(int j=0; j<m.vertexCount(); j++)
	        	{
	        		m.vertices[j].label = j;
	        	}
	        	
	        	m.ComputeVertexNormals();

	        }

	        #endregion

		}
		
		public void LoadDataObjectsFromIDF(string fileName)
		{
			DataObject dObj = null;
			
			using(TextReader sr = new StreamReader(fileName))
	        {
				string line;
	        	while((line = sr.ReadLine()) != null)
	        	{
	        		string[] lineSplit = line.Split('#');
	        		if(lineSplit[0] == "p")
	        		{
	        			if(lineSplit.Length == 5)	//it's the location
	        			{
	        				double x = System.Convert.ToDouble(lineSplit[2]);
	        				double y = System.Convert.ToDouble(lineSplit[3]);
	        				double z = System.Convert.ToDouble(lineSplit[4]);

	        				dObj = new DataObject(lineSplit[1], new Vector(x, y, z));
	        				dataObjects.Add(dObj);

	        			}
	        			else if(lineSplit.Length < 5)	//it's a parameter
	        			{
	        				dObj.parameters.Add(lineSplit[2]);
	        				if(lineSplit.Length == 4)
	        				{
	        					dObj.parameter_values.Add(lineSplit[3]);
	        				}
	        				else{
	        					dObj.parameter_values.Add("");
	        				}
	        			}
	        			
	        		}
	        	}
			}
		}
	}
	public class DataObject
	{
		public List<string> parameters;
		public List<string> parameter_values;
		public Vector loc;
		public string name;
		
		public DataObject(string oName, Vector location)
		{
			loc = location;
			name = oName;
		}
	}
}
