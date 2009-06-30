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
	public class OBJReader
	{
		public OBJReader(string fileName)
		{
			
		}
		
		public static List<Mesh> LoadMeshesFromOBJ(string fileName)
		{
			List<Mesh> meshes = new List<Mesh>();	//a list to hold the meshes
			int meshCount = 0;
			
			int vertCounter 		= 0;
			List<Vector> verts 		= new List<Vector>();	//one big array to hold all the verts
			List<Vector> textCoords = new List<Vector>();	//another to hold the texture coords
			List<Vector> norms		= new List<Vector>();	//and one more to hold the normals
			
			ArrayList faceIndices	= new ArrayList();
			List<int> innerFaceList = new List<int>();
			ArrayList textIndices		= new ArrayList();
			List<int> innerTextIndices	= new List<int>();
			ArrayList normIndices		= new ArrayList();
			List<int> innerNormIndices	= new List<int>();
			
			#region read file
	        using(TextReader sr = new StreamReader(fileName))
	        {
	        	string line;
	        	while((line = sr.ReadLine()) != null)
	        	{
	        		string[] lineSplit = line.Split(' ');
	        		
	        		if(lineSplit[0] == "g")
	        		{	
	        			if(meshCount == 0)
	        			{
							faceIndices.Add(innerFaceList);		//add the first set of sub lists
	        				textIndices.Add(innerTextIndices);
	        				normIndices.Add(innerNormIndices);
	        			}
	        			else
	        			{
	        				faceIndices.Add(innerFaceList);				//add the previous one
	        				textIndices.Add(innerTextIndices);
	        				normIndices.Add(innerNormIndices);
	        				
	        				innerTextIndices = new List<int>();
	        				innerNormIndices = new List<int>();
	        				innerFaceList = new List<int>();	//make a new one
	        				
	        				faceIndices.Add(innerFaceList);		//add the one you've just created
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
		        				innerFaceList.Add(Convert.ToInt16(innerSplit[0]));
		        				
		        				if(innerSplit[1] != "")
		        				{
		        					innerTextIndices.Add(Convert.ToInt16(innerSplit[1]));
		        				}
		        				
		        				if(innerSplit.Length == 3)
		        				{
		        					if(innerSplit[2] != "")
		        					{
		        						innerNormIndices.Add(Convert.ToInt16(innerSplit[2]));
		        					}
		        				}
		        			}
		        			else	//just a
		        			{
		        				innerFaceList.Add(Convert.ToInt16(innerSplit[0]));
		        			}
	        			}

	        		}
	        		else if(lineSplit[0] == "vt")
	        		{
	        			Vector vt = new Vector(Convert.ToDouble(lineSplit[1]),
	        			                      Convert.ToDouble(lineSplit[2]),
	        			                      Convert.ToDouble(0.0));
	        			textCoords.Add(vt);
	        		}
	        	}
	        }
	        #endregion
	        
	        #region build meshes
	        for(int i=0; i<faceIndices.Count; i++)	//loop through the entire arrayList of lists
	        {

	        	List<int> f = faceIndices[i] as List<int>;	//the curr face index list 
	        	List<int> t = textIndices[i] as List<int>;	//the curr texture coordinate index list
	        	List<int> n = normIndices[i] as List<int>;	//the curr normals list - this doesn't matter yet...it's empty
	        	
	        	//stride by three creating new triangular meshes
	        	Mesh m = new Mesh();
	        	meshes.Add(m);
	        	
	        	//create the verts
	        	for(int j=0; j<verts.Count;j++)	//put all the verts in there
	        	{
	        		m.AddVertex(verts[j]);	//add the vertex by the face index
//	        		Debug.WriteLine("Adding vertex for " + verts[j].x +":" + verts[j].y + ":" + verts[j].z);
	        	}
	        	
	        	//create the texture coords
	        	for(int j=0; j<m.vertexCount() ;j++)
	        	{
	        		m.vertices[j].tex = textCoords[j];
	        	}
	        	//add the faces
	        	for(int j=0; j<f.Count-2; j+=3)
	        	{
	        		
	        		m.AddTriangle(f[j]-1, f[j+1]-1, f[j+2]-1);
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
	        
	        return meshes;
	        
		}
		
	}
}
