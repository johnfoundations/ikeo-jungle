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
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

using System.Windows.Forms;
using System.Diagnostics;

using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Math;
using OpenTK.Platform.Windows;

using BuroHappold.Analysis;
using BuroHappold.Analysis.Elements;
using Vector3 = OpenTK.Math.Vector3;	//resolve class conflict with BHAnalysis
using BuroHappold.Interface;

using ikeo;

namespace ikeo
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		bool _loaded = false;
		
		#region file loading and updating
		BuroHappold.Interface.BHXml_Parser _bhxml = new BHXml_Parser();
		bool _modelLoaded = false;	//check for form loading
		bool _imageLoaded = false;
		string _texturePath;
		#endregion
		
		#region text
//		TextureFont _sans = new TextureFont(new Font(FontFamily.GenericSansSerif, 12.0f));
		Font _sans = new System.Drawing.Font(FontFamily.GenericSansSerif, 12.0f);
		ITextPrinter _text = new TextPrinter();

		float _lineHeight = 20.0f;
		
		#endregion
		
		#region HUD info
		int _totalVerticesInScene	= 0;
		int _totalFacesInScene 		= 0;
		#endregion
		
		#region animation
		string _fps = "";
		float rotation = 0;
		Stopwatch sw = new Stopwatch();
		int idleCounter = 0;
		double accumulator = 0;
		#endregion
		
		#region drawing settings
		float _lineSize = 1.0f;				//gl line render size
		float _normalSize = .25f;
		float _pointSize = 7.0f;			//gl node render size
		float _red = 0.0f;					//gl red color
		float _green = 0.0f;				//gl green color
		float _blue = 0.0f;					//gl blue color;
//		float _alpha = 1.0f;				//gl alpha color;
		
		bool _drawWireFrame 	= false;	//always start viewing the faces		
//		bool _drawFaces 		= true;
		bool _drawClipPlanes 	= false;
		bool _drawNormals 		= false;
		bool _drawImageTexture 	= true;
		bool _reverseNormals 	= true;
		bool _drawVertices 		= false;
		bool _drawNodeNumbers 	= false;
		bool _drawHUD			= true;
		bool _lightsOn			= true;
		bool _zoomToFit			= false;
		bool _drawGrid			= true;
		#endregion
		
		#region global object/display lists
		
		List<BHBar> _barList 		= new List<BHBar>();		//global list for all bars and lines
		List<BHNode> _nodeList 		= new List<BHNode>();		//global list for all nodes
		List<BHPlate> _plateList 	= new List<BHPlate>();		//global list for all plates
		List<Mesh> _meshList		= new List<Mesh>();			//global list for all meshes
		List<float[]> _meshVerts	= new List<float[]>();		//list for mesh vertex arrays
		List<int[]> _indices		= new List<int[]>();		//list for mesh vertex index arrays
		List<float[]> _textCoords	= new List<float[]>();		//list for the texture coords
		List<float[]> _meshNormals	= new List<float[]>();		//list for the mesh normals
		
		
		private int _faces; 		// display list for all faces
		private int[] _pointInd;
		private float[] _points;
		
        #endregion
		
		#region rotation/zoom/pan
		private System.Object matrixLock = new System.Object();
        private Arcball arcBall = new Arcball(640.0f, 480.0f);
		private float[] matrix = new float[16];
        private Matrix4f LastTransformation = new Matrix4f();
        private Matrix4f ThisTransformation = new Matrix4f();
        #endregion
        
        #region clipping planes
        ClipPlane _cl0;
//        ClipPlane _cl_xy;
//        ClipPlane _cl_yz;
//        ClipPlane _cl_xz;
        #endregion
       	
        #region lighting
		private static float[] _LightAmbient = new float[] {
						0.0F,
						0.0F,
						0.0F,
						1F};
		private static float[] _LightDiffuse = new float[] {
						1F,
						1F,
						1F,
						0.0F};
		private static float[] _LightPosition = new float[] {
						0.0F,
						0.0F,
						0.0F,
						1.0F};
		private static float[] _LightSpecular = new float[] {
						1F,
						1F,
						1F,
						1F};

		#endregion

		#region mouse
        private Point mouseStartDrag;
        private static bool isLeftDrag = false;
        private static bool isRightDrag = false;
        private static bool isMiddleDrag = false;
		#endregion
		
		#region matrix info setup
		double[] _modelViewMatrix = new double[0];
		double[] _projectionMatrix = new double[0];
		int[] _viewport = new int[0];
	    Vector3 _objPoint 				= new Vector3(0.0f, 0.0f, 0.0f);
	    Vector3 _winPoint 				= new Vector3(0f,0f,0f);
	    Vector3 _worldPoint 			= new Vector3(0f,0f,0f);
		#endregion
		
		#region texturing
		Bitmap _bitmap;
		int _texture;
		#endregion
		
		OpenTK.Math.Vector3 sceneCenter = new Vector3();
		Vector3 _sceneMax = new Vector3();
		
		/// <summary>
		/// Program entry point.
		/// </summary>
		public MainForm()
		{
			InitializeComponent();

		}
		
		/// <summary>
		/// On form load establish event handlers, repaint, and set identity matrices
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void MainFormLoad(object sender, EventArgs e)
		{
			_loaded = true;							//make sure the form is loaded
			
//			this.WindowState = FormWindowState.Maximized;	//maximize the window
			
			glControl1.SwapBuffers();

			glControl1.Load += new EventHandler(GlControlLoad);
			glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(glOnMouseMove);
			glControl1.SizeChanged += new EventHandler(GlControl1Resize);
			glControl1.KeyDown += new KeyEventHandler(glControl1KeyDown);	//handle key presses
			
			GlControl1Resize(sender, e);
			
			//arcball
			LastTransformation.SetIdentity(); // Reset Rotation
            ThisTransformation.SetIdentity(); // Reset Rotation
            ThisTransformation.get_Renamed(matrix);
            
            Application.Idle +=	new EventHandler(Application_Idle);

			sw.Start();		//start your stopwatch
			
			#region mouse handles
            MouseControl mouseControl = new MouseControl(glControl1);
            mouseControl.AddControl(glControl1);
            mouseControl.LeftMouseDown += new MouseEventHandler(glOnLeftMouseDown);
            mouseControl.LeftMouseUp += new MouseEventHandler(glOnLeftMouseUp);
            mouseControl.RightMouseDown += new MouseEventHandler(glOnRightMouseDown);
            mouseControl.RightMouseUp += new MouseEventHandler(glOnRightMouseUp);
            mouseControl.MiddleMouseDown += new MouseEventHandler(glOnMiddleMouseDown);
            mouseControl.MiddleMouseUp += new MouseEventHandler(glOnMiddleMouseUp);
      
            #endregion
		
		}
		
		/// <summary>
		/// On form load - draw the gl content.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GlControlLoad(object sender, EventArgs e)
		{
			PlotGL();	//on load - draw the gl content
		}
		
		private void glControl1KeyDown(object sender, KeyEventArgs e)
		{
			if(!_loaded)
				return;
			if(e.KeyCode == Keys.N)
			{
				if(_drawNormals == true) _drawNormals = false;
				else _drawNormals = true;
			}
			
			if(e.KeyCode == Keys.V)
			{
				if(_drawVertices == true) _drawVertices = false;
				else _drawVertices = true;
			}
			
			if(e.KeyCode == Keys.T)
			{
				if(_drawImageTexture == true) _drawImageTexture = false;
				else _drawImageTexture = true;
			}
			
			if(e.KeyCode == Keys.D3)
			{
				if(_drawNodeNumbers == true) _drawNodeNumbers = false;
				else _drawNodeNumbers = true;
			}
			
			if(e.KeyCode == Keys.L)
			{
				if(_lightsOn == true) 
				{
					_lightsOn = false;
					GL.Disable(EnableCap.Lighting);
				}
				else{
					GL.Enable(EnableCap.Lighting);
				}
			}
			
			if(e.KeyCode == Keys.C)
			{
				if(_drawClipPlanes == true) _drawClipPlanes = false;
				else _drawClipPlanes = true;
			}
			
			if(e.KeyCode == Keys.W)
			{
				if(_drawWireFrame == true) _drawWireFrame = false;
				else _drawWireFrame = true;
			}
			
			if(e.KeyCode == Keys.G)
			{
				if(_drawGrid == true) _drawGrid = false;
				else _drawGrid = true;
			}
			
//			if(e.KeyCode == Keys.Oemplus)
//			{
//				_cl0.norm.Z += 0.1f;
//			}
//			if(e.KeyCode == Keys.F)
//			{
//				_zoomToFit = true;
//			}
			
		}
		
		#region Compute frame rate, redraw continuously
		/// <summary>
		/// On application idle - 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Application_Idle(object sender, EventArgs e)
		{
			double milliseconds = ComputeTimeSlice();
      		Accumulate(milliseconds);
      		Animate(milliseconds);
		}
		
		/// <summary>
		/// Compute elapsed time on stopwatch
		/// </summary>
		/// <returns></returns>
		private double ComputeTimeSlice()
		{
			sw.Stop();			//measured since last idle run
			double timeslice = sw.Elapsed.TotalMilliseconds;
			sw.Reset();			//set the stopwatch back to zero
			sw.Start();			//restart the stopwatch
			return timeslice;
			
		}
		
		/// <summary>
		/// Invalidate the control and redraw
		/// </summary>
		/// <param name="milliseconds"></param>
		private void Animate(double milliseconds)
		{
			float deltaRotation = (float)milliseconds / 20.0f;
			rotation += deltaRotation;
			glControl1.Invalidate();
		}
		
		private void Accumulate(double milliseconds)
		{
			idleCounter++;
			accumulator += milliseconds;
			if (accumulator > 1000)
			{
				_fps = idleCounter.ToString();
				accumulator -= 1000;
				idleCounter = 0; // don't forget to reset the counter!
			}
		}
		#endregion
		
		/// <summary>
		/// On repaint - draw the gl content.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void GlControl1Paint(object sender, PaintEventArgs e)
		{
			if (!_loaded)	//check for loading
				return;
			
			PlotGL();	
			
		}
		
		/// <summary>
		/// Setup the gl context, clearing buffers and resetting matrices.
		/// </summary>
		private void SetupViewport()
		{

			int w = viewport1.Width;
			int h = viewport1.Height;
			
			glControl1.Width = w;
			glControl1.Height = h;
			glControl1.Location = new Point(0,0);
			
			//move the viewport to only be part of the screen
//			this.viewport1.Location = new Point(200, 25);
//			this.viewport1.Width = Size.Width - 200;
//			this.viewport1.Height = Size.Height - 25;
			
            GL.ShadeModel(ShadingModel.Smooth);								// enable smooth shading
            
            GL.Enable(EnableCap.DepthTest);									// enables depth testing
//            GL.ClearDepth(1.0f);											// depth buffer setup
            GL.Enable(EnableCap.PointSmooth);								// point smoothing
//            GL.Enable(EnableCap.LineSmooth);								//enable line smoothing
//            GL.Enable(EnableCap.CullFace);									//enable face culling
			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);	// nice perspective calculations
			
			GL.ClearColor(Color.LightBlue);									// black to black
			
            GL.Viewport(0, 0,
                        viewport1.Width, 
                        viewport1.Height);
            
            GL.PushMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Glu.Perspective(60.0, (double)this.glControl1.Width / (double)this.glControl1.Height, 1.0, 10000.0);
            
            #region lighting
            GL.Enable(EnableCap.Lighting);                                      // Enable Lighting
			GL.Enable(EnableCap.Light0);
            
			GL.Light(LightName.Light0, LightParameter.Diffuse, _LightDiffuse);
            GL.Light(LightName.Light0, LightParameter.Position, _LightPosition);
            GL.Light(LightName.Light0, LightParameter.Specular, _LightSpecular);
			#endregion	//set this light in eye coordinates
			
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();

            #region material
            GL.Enable(EnableCap.ColorMaterial);	 // Enable Color Material
            GL.ColorMaterial(MaterialFace.Front,ColorMaterialParameter.Diffuse);	//.AmbientAndDiffuse);
            GL.FrontFace(FrontFaceDirection.Cw);	//this determines winding for all polys

            #endregion

            #region blending
//			GL.Disable(EnableCap.Blend);
            #endregion blending

            GL.PolygonOffset(1.0f, 1.0f);
            GL.EnableClientState(EnableCap.PolygonOffsetFill);

            arcBall.setBounds((float)glControl1.Width, (float)glControl1.Height); // Update mouse bounds for arcball
            PlotGL();
		}
		
		/// <summary>
		/// On resize of form - call setup method and invalidate the form.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void GlControl1Resize(object sender, EventArgs e)
		{
			SetupViewport();
			glControl1.Invalidate();
		}
		
		/// <summary>
		/// Setup the initial vertex array
		/// </summary>
		private void SetupVertexArray()
		{
			//TODO: add functionality so no rebuild every frame
			
			List<int> vertIndexList = new List<int>();	//a list for the vert inidices

			//go through all the bars and put their nodal indices
			//into the vertex array index array
			foreach (BHBar b in _barList)
			{
				int nodeIndex1 = _nodeList.IndexOf(b.StartNode);
				int nodeIndex2 = _nodeList.IndexOf(b.EndNode);
				
				vertIndexList.Add(nodeIndex1);
				vertIndexList.Add(nodeIndex2);
			}

			_pointInd = vertIndexList.ToArray();		//convert the list to an array
			_points = new float[_nodeList.Count*3];		//create an array for the point information 
			
		}
		
		private void SetupMeshVertexArrays()
		{
			int vCount = 0;
			float avgX = 0.0f;
			float avgY = 0.0f;
			float avgZ = 0.0f;
			
			foreach(Mesh m in _meshList)
			{
				float[]verts 	= new float[m.vertexCount()*3];
				int[]ind		= new int[m.faceCount()*3];		//face indices only - drop the textCoord and norm indices
				float[]text		= new float[m.vertexCount()*2];
				float[]norms	= new float[m.vertexCount()*3];
				
				int vertCount = 0;
				int textCount = 0;
				int indCount = 0;
				int normCount = 0;
				
				//fill up the verts array
				for(int i=0; i<m.vertexCount(); i++)
				{
					verts[vertCount] 	= (float)m.vertices[i].v.x;
					verts[vertCount+1] 	= (float)m.vertices[i].v.y;
					verts[vertCount+2] 	= (float)m.vertices[i].v.z;
					vertCount += 3;
					
					_totalVerticesInScene++;	//keep track of this stuff
					
					norms[normCount] 	= (float)m.vertices[i].normal.x;
					norms[normCount+1] 	= (float)m.vertices[i].normal.y;
					norms[normCount+2] 	= (float)m.vertices[i].normal.z;
//					Debug.WriteLine("Setup normal:" + norms[normCount] + "," + norms[normCount+1] + "," 
//					                + norms[normCount+2]);
					normCount+=3;
					
					
					if(Math.Abs((float)m.vertices[i].v.x) > _sceneMax.X)
					{
						_sceneMax.X = (float)m.vertices[i].v.x;
					}
					if(Math.Abs((float)m.vertices[i].v.y) > _sceneMax.Y)
					{
						_sceneMax.Y = (float)m.vertices[i].v.y;
					}
					if(Math.Abs((float)m.vertices[i].v.z) > _sceneMax.Z);
					{
						_sceneMax.Z = (float)m.vertices[i].v.z;
					}
					
					avgX += (float)m.vertices[i].v.x;
					avgY += (float)m.vertices[i].v.y;
					avgZ += (float)m.vertices[i].v.z;
					vCount++;
					
				}
				
				//fill up the text coords array
				for(int i=0; i<m.vertexCount(); i++)
				{
					text[textCount] 	= (float)m.vertices[i].tex.x;
					text[textCount + 1] = 1.0f - (float)m.vertices[i].tex.y;	//reverse this for windows
					textCount += 2;
				}
				
				//fill up the faces index array
				for(int i=0; i<m.faceCount(); i++)
				{
					ind[indCount] = m.faces[i].vertices[0].label;
					ind[indCount+1] = m.faces[i].vertices[1].label;
					ind[indCount+2] = m.faces[i].vertices[2].label;
					
					indCount += 3;
					
					_totalFacesInScene ++;
				}
				
				//add all the arrays to the lists
				_meshVerts.Add(verts);
				_indices.Add(ind);
				_textCoords.Add(text);
				_meshNormals.Add(norms);
				
			}
			
			sceneCenter.X = avgX/vCount;
			sceneCenter.Y = avgY/vCount;
			sceneCenter.Z = avgZ/vCount;
		}
		
		private void SetupVertexBufferObjects()
		{
			
		}
		
		private void ReverseNormals()
		{
			foreach (Mesh m in _meshList)
			{
				m.ReverseNormals();
			}
			
			_reverseNormals = false;	//turn it off
		}
		
		private void SetupClipPlanes()
		{
		
			Vector3 a = sceneCenter - new Vector3(sceneCenter.X, sceneCenter.Y + 1.0f, sceneCenter.Z);
			Vector3 b = sceneCenter - new Vector3(sceneCenter.X + 1.0f, sceneCenter.Y, sceneCenter.Z);
			
			a = Vector3.Normalize(a);
			b = Vector3.Normalize(b);
//			Vector3 a = new Vector3(0.0f, 1.0f, 0.0f);
//			Vector3 b = new Vector3(1.0f, 0.0f, 0.0f);
			
			_cl0 = new ClipPlane(a,b, sceneCenter);


		}
		
		/// <summary>
		/// Rebuild the vertex array for drawing all lines and points
		/// </summary>
		private void RebuildVertexArray()
		{

			float[] rebuiltVerts = new float[_points.Length];
			
			int vertCount = 0;
			float avgX = 0.0f;
			float avgY = 0.0f;
			float avgZ = 0.0f;
			
			for (int i=0; i < this._nodeList.Count; i++)
			{
				BHNode n = this._nodeList[i] as BHNode;
				
				avgX += (float)n.X;
				avgY += (float)n.Y;
				avgZ += (float)n.Z;
				
				vertCount ++;
			}

			sceneCenter.X = avgX/vertCount;
			sceneCenter.Y = avgZ/vertCount;
			sceneCenter.Z = avgY/vertCount;
			
			vertCount = 0;
			
			for (int i=0; i < this._nodeList.Count; i++)
			{

				BHNode n = this._nodeList[i] as BHNode;
				
				
				rebuiltVerts[vertCount] 	= (float)n.X - sceneCenter.X;	//move back to center - for the arcball
				rebuiltVerts[vertCount+1]	= (float)n.Z - sceneCenter.Y;	//flip the y and z for openGl
				rebuiltVerts[vertCount+2] 	= (float)n.Y - sceneCenter.Z;

				vertCount += 3;
			}

			_points = rebuiltVerts;	//change out the old for the new
		}
		
		/// <summary>
		/// Zoom the camera to show all objects
		/// </summary>
		private void ZoomToFill()
		{
			//zoom to fit the scene to the bounding sphere
			float rad = 0.0f;
			if(sceneCenter.X > rad) rad = sceneCenter.X;
			if(sceneCenter.Y > rad) rad = sceneCenter.Y;
			if(sceneCenter.Z > rad) rad = sceneCenter.Z;

			Glu.LookAt(0.0f, 0.0f, rad, sceneCenter.X, sceneCenter.Y, sceneCenter.Z, 0.0f, 1.0f, 0.0f);
			_zoomToFit = false;	//turn it off
		}
		
		
		/// <summary>
		/// Draw lines from a vertex array
		/// </summary>
		private void DrawLines()
		{

			GL.PushMatrix();                 // NEW: Prepare Dynamic Transform
          	GL.MultMatrix(matrix);           // NEW: Apply Dynamic Transform
			
			GL.Color3(_red, _green, _blue);
            GL.LineWidth(_lineSize);
            GL.EnableClientState(EnableCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 0, _points);
            GL.DrawElements(BeginMode.Lines, _pointInd.Length, DrawElementsType.UnsignedInt, _pointInd);
           	GL.DisableClientState(EnableCap.VertexArray);

           	GL.PopMatrix(); // NEW: Unapply Dynamic Transform
           	
           	
		}
		
		private void DrawPoints()
		{
			GL.PushMatrix();                 // NEW: Prepare Dynamic Transform
            GL.MultMatrix(matrix);           // NEW: Apply Dynamic Transform
            GL.Translate(0.0f-sceneCenter.X, 0.0f-sceneCenter.Y, 0.0f-sceneCenter.Z);
            
            GL.PushAttrib(AttribMask.CurrentBit);
            GL.Disable(EnableCap.Lighting);
			GL.Color3(Color.Yellow);	//draw black points
            GL.PointSize(_pointSize);
            GL.Begin(BeginMode.Points);
            foreach(Mesh m in _meshList)
            {
            	for(int i=0; i<m.vertexCount(); i++)
            	{
            		Vector v = m.vertices[i].v;
            		GL.Vertex3(v.x, v.y, v.z);
            	}
            	
            }
            
            GL.End();
           	GL.PopAttrib();
           	GL.Enable(EnableCap.Lighting);
           	GL.PopMatrix(); // NEW: Unapply Dynamic Transform
		}
		
		private void DrawNormals()
		{
			GL.PushMatrix();                 // NEW: Prepare Dynamic Transform
          	GL.MultMatrix(matrix);           // NEW: Apply Dynamic Transform
			GL.Translate(0.0f-sceneCenter.X, 0.0f-sceneCenter.Y, 0.0f-sceneCenter.Z);
			
			GL.Disable(EnableCap.Lighting);
			GL.PushAttrib(AttribMask.CurrentBit);
			GL.Color3(Color.White);
            GL.LineWidth(_normalSize);
            
            GL.Begin(BeginMode.Lines);
            
            foreach(Mesh m in _meshList)
            {

            	for(int i=0; i<m.vertexCount(); i++)
            	{
            		Vector v1 = m.vertices[i].v;
            		Vector v2 = m.vertices[i].normal + v1;
            		
            		GL.Vertex3(v1.x, v1.y, v1.z);
            		GL.Vertex3(v2.x, v2.y, v2.z);
            	}
            }
            
            GL.End();
            GL.PopAttrib();
            GL.Enable(EnableCap.Lighting);
           	GL.PopMatrix(); // NEW: Unapply Dynamic Transform
		}
		
		private void DrawMeshesSolid()	//bool clipTest)
		{
			
			GL.PushMatrix();
	        GL.MultMatrix(matrix);
	        GL.Translate(0.0f-sceneCenter.X, 0.0f-sceneCenter.Y, 0.0f-sceneCenter.Z);

			GL.EnableClientState(EnableCap.VertexArray);
			GL.EnableClientState(EnableCap.NormalArray);
			
			if(_drawImageTexture)
			{
				GL.EnableClientState(EnableCap.TextureCoordArray);
			}
			
			for(int i=0; i<_meshVerts.Count; i++)	//pick one of the lists to use as the count
			{

				float[] v 	= _meshVerts[i];
				float[] t 	= _textCoords[i];
				int[] ind	= _indices[i];
				float[] n	= _meshNormals[i];
				
				GL.NormalPointer(NormalPointerType.Float, 0, n);
				GL.VertexPointer(3, VertexPointerType.Float, 0, v);

				if(_drawImageTexture)
				{
					if(_imageLoaded)
					{
						GL.Enable(EnableCap.Texture2D);
						GL.BindTexture(TextureTarget.Texture2D, _texture);
		           		GL.TexCoordPointer(2, TexCoordPointerType.Float, 0 ,t);	
					}
				}
				
				//DRAW FACES
				GL.Color3(Color.LightBlue);
				GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
				GL.DrawElements(BeginMode.Triangles,ind.Length, DrawElementsType.UnsignedInt, ind);

			}
			
			if(_imageLoaded)
			{
				GL.DisableClientState(EnableCap.TextureCoordArray);
				GL.Disable(EnableCap.Texture2D);
			}
			
			GL.DisableClientState(EnableCap.NormalArray);
			GL.DisableClientState(EnableCap.VertexArray);

			GL.PopMatrix();
		}
		
		private void DrawMeshesWire()
		{
			GL.PushMatrix();
	        GL.MultMatrix(matrix);
	        GL.Translate(0.0f-sceneCenter.X, 0.0f-sceneCenter.Y, 0.0f-sceneCenter.Z);

			GL.EnableClientState(EnableCap.VertexArray);
			GL.EnableClientState(EnableCap.NormalArray);
			
			if(_drawImageTexture)
			{
				GL.EnableClientState(EnableCap.TextureCoordArray);
			}
			
			for(int i=0; i<_meshVerts.Count; i++)	//pick one of the lists to use as the count
			{

				float[] v 	= _meshVerts[i];
				float[] t 	= _textCoords[i];
				int[] ind	= _indices[i];
				float[] n	= _meshNormals[i];
				
				GL.NormalPointer(NormalPointerType.Float, 0, n);
				GL.VertexPointer(3, VertexPointerType.Float, 0, v);
				
				//TURN OFF LIGHTS FOR POINTS AND LINES
				GL.Disable(EnableCap.Lighting);
				GL.PushAttrib(AttribMask.CurrentBit);
				
				//DRAW WIREFRAME
				GL.LineWidth(.5f);
				GL.Color3(Color.Black);
				GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
				GL.DrawElements(BeginMode.Triangles,ind.Length, DrawElementsType.UnsignedInt, ind);
				
				GL.PopAttrib();
				//TURN LIGHTS BACK ON
				GL.Enable(EnableCap.Lighting);
				
			}
			
			GL.DisableClientState(EnableCap.NormalArray);
			GL.DisableClientState(EnableCap.VertexArray);

			GL.PopMatrix();
		}
		
		/// <summary>
		/// Draw node numbers
		/// </summary>
		private void DrawNodeNumbers()
		{

            GL.PushMatrix();                 // NEW: Prepare Dynamic Transform
            GL.MultMatrix(matrix);           // NEW: Apply Dynamic Transform
            GL.Translate(0.0f-sceneCenter.X, 0.0f-sceneCenter.Y, 0.0f-sceneCenter.Z);
            GL.Disable(EnableCap.Lighting);
            int vCount = 0;
            
//           foreach (BHNode n in _nodeList)
			foreach(Mesh m in _meshList)
              {
				for(int i=0; i<m.vertexCount(); i++)
				{
		             double[] modelViewMatrix = new double[16];
		             double[] projectionMatrix = new double[16];
		             int[] viewport = new int[4];
		             
//		             Vector3 objPoint = new Vector3((float)n.X, 
//		                                             (float)n.Z,
//		                                             (float)n.Y);
		             Vector3 objPoint = new Vector3((float)m.vertices[i].v.x,
		                                            (float)m.vertices[i].v.y,
		                                            (float)m.vertices[i].v.z);
		             
		             Vector3 winPoint = new Vector3(0f,0f,0f);
		              
		             GL.GetDouble(GetPName.ModelviewMatrix, modelViewMatrix);
		             GL.GetDouble(GetPName.ProjectionMatrix,projectionMatrix);
		             GL.GetInteger(GetPName.Viewport, viewport);
		             
		             OpenTK.Graphics.Glu.Project(objPoint, modelViewMatrix,
		                                          projectionMatrix,
		                                          viewport,
		                                          out winPoint);
		           
		           //text.Begin() sets up and orthogonal matrix with x->view.width (left->right)
		           //y->view.height (top->bottom), then resets it after drawing
		           _text.Begin();
	            	
//		           GL.Color3(Color.PaleGoldenrod);
	            	//translate has a weird winY that needs to be further translated by viewport[3]
	            	GL.Translate(winPoint.X, viewport[3] - winPoint.Y, 0.0f);
//	           	 	_text.Draw(n.Id.ToString(), _sans);
	            	_text.Print(vCount.ToString(), _sans, Color.Black);
	
	            	_text.End();
            	
            		vCount++;
				}

               }
			GL.Enable(EnableCap.Lighting);
			GL.PopMatrix(); // NEW: Unapply Dynamic Transform
		}
		
		private void DrawHUD()
		{

			GL.PushMatrix();
			
			// switch to projection mode
			GL.MatrixMode(MatrixMode.Projection);
			// save previous matrix which contains the
			//settings for the perspective projection
			GL.PushMatrix();
			// reset matrix
			GL.LoadIdentity();
			
			// set a 2D orthographic projection
			Glu.Ortho2D(0, glControl1.Width, 0, glControl1.Height);

			GL.MatrixMode(MatrixMode.Modelview);
			
			GL.PushMatrix();
			GL.LoadIdentity();
			
			//draw text here
			GL.Disable(EnableCap.Lighting);
			GL.Disable(EnableCap.ClipPlane0);
			
			_text.Begin();
			GL.Translate(10.0f, 10.0f, 0.0f);
			_text.Print("Total vertices \t: " + _totalVerticesInScene.ToString(), _sans, Color.LightGray);
			_text.End();
			_text.Begin();
			GL.Translate(10.0f, 10.0f + _lineHeight, 0.0f);
			_text.Print("Total faces \t: " + _totalFacesInScene.ToString(), _sans, Color.LightGray);
			_text.End();
//			_text.Begin();
//			GL.Translate(10.0f, 30.0f + _lineHeight, 0.0f);
//			_text.Draw("FPS \t: " + _fps, _sans);
//			_text.End();

			GL.Enable(EnableCap.Lighting);
			GL.Enable(EnableCap.ClipPlane0);
			
			GL.MatrixMode(MatrixMode.Projection);
			GL.PopMatrix();
			GL.MatrixMode(MatrixMode.Modelview);
			GL.PopMatrix();
			
			
		}
		
		/// <summary>
        /// All openGl drawing commands go here
        /// </summary>
        public void PlotGL()
        {
            try
            {
                lock (matrixLock)
                {
                    ThisTransformation.get_Renamed(matrix);
                }

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit); // Clear screen and DepthBuffer

                GL.LoadIdentity();
				
				DrawBackground();

              	//look at the scene center
              	//TODO: back this off by the distance
              	//of the scene bounding sphere
              	if(_zoomToFit)
              	{
              		ZoomToFill();
              	}
              	else{
					Glu.LookAt(0.0, 0.0, +100.0,
	              	           0.0,0.0,0.0,
	              	           0.0, 1.0, 0.0);
              	}
              	
                #region plot all diplay lists
    			
                if(_drawGrid) DrawGrid();
          
                DrawAxes();
                
                if (_modelLoaded)
                {
                	//calc distance to scene center
                	double dist	= 	Math.Sqrt(Math.Pow(sceneCenter.X,2) + 
              						Math.Pow(sceneCenter.Y,2) + 
              						Math.Pow(sceneCenter.Z,2));

					
                	#region clip planes
                	if(_drawClipPlanes)
                	{
	                	GL.Enable(EnableCap.ClipPlane0);
		                DrawClipPlanes();
	
		                GL.Enable(EnableCap.StencilTest);			//turn on stencil testing
		                GL.Clear(ClearBufferMask.StencilBufferBit);
		                GL.Disable(EnableCap.DepthTest);			//disable depth testing
		                GL.ColorMask(false, false, false, false);	//turn off writes to color buffer
		                
		                GL.Enable(EnableCap.CullFace);									//enable face culling
		                //first pass
		                GL.StencilFunc(StencilFunction.Always, 0, 0);					//set stencil function
		                GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Incr);	//increment by one for non-black pixels
		                GL.CullFace(CullFaceMode.Front);								//render back faces only
		                DrawMeshesSolid();													//render
		                //second pass
		                GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Decr);	//decrement by one for non-black pixels
		                GL.CullFace(CullFaceMode.Back);									//render the front faces only
		                DrawMeshesSolid();													//render
		                GL.Disable(EnableCap.CullFace);									//disable face culling
		                
		                //draw the clip quads
		                GL.ColorMask(true, true, true, true);							//turn on color writing
		                GL.Enable(EnableCap.DepthTest);									//enable depth testing
		                GL.Disable(EnableCap.ClipPlane0);								//disable the clip plane
		                GL.StencilFunc(StencilFunction.Notequal,0, ~0);					//draw all pixels that are equal to one
		                DrawClipQuads();
	
		                GL.Disable(EnableCap.StencilTest);								//turn off stencil testing
		                GL.Enable(EnableCap.ClipPlane0);								//turn the clip plane back on
                	}
                	else{
                		GL.Disable(EnableCap.ClipPlane0);
                	}
                	#endregion
                	
                	#region meshes
                	GL.Enable(EnableCap.CullFace);
                	GL.CullFace(CullFaceMode.Back);
                	if(!_drawWireFrame)DrawMeshesSolid();
	                DrawMeshesWire();
	                GL.Disable(EnableCap.CullFace);
	                #endregion

	                #region normals
	                if(_reverseNormals)
					{
						ReverseNormals();
					}
	                
	                if(_drawNormals)
	                {
	                	DrawNormals();
	                }
                	#endregion
                	
                	#region draw curves
                	#endregion
                	
//	                DrawLines();			//draw the lines
                	
                	#region vertices
                	if(_drawVertices)
                	{
	              		DrawPoints();			//draw the points
                	}
					#endregion
					
                	#region node numbers
                	if(_drawNodeNumbers)
                	{
                		DrawNodeNumbers();
                	}
                	#endregion
                }
                
                #endregion 

                #region draw HUD
                if(_drawHUD)
                {
               		DrawHUD();
                }
                #endregion
                
              	GL.Flush();     			// Flush the GL Rendering Pipeline
                glControl1.SwapBuffers();	//YOU NEED THIS FOR OPENTK!!!

            }
            catch(Exception e)
            {
            	MessageBox.Show(e.StackTrace);
                return;
            }

        }
        
		#region Mouse Control
        private void startDrag(Point MousePt)
        {
            lock (matrixLock)
            {
                LastTransformation.set_Renamed(ThisTransformation); // Set Last Static Rotation To Last Dynamic One
            }
            arcBall.click(MousePt); // Update Start Vector And Prepare For Dragging

            mouseStartDrag = MousePt;

        }

        private void drag(Point MousePt)
        {
            Quat4f ThisQuat = new Quat4f();

            arcBall.drag(MousePt, ThisQuat); // Update End Vector And Get Rotation As Quaternion

            lock (matrixLock)
            {
                if (isMiddleDrag) //zoom
                {
                    double len = Math.Sqrt(mouseStartDrag.X * mouseStartDrag.X + mouseStartDrag.Y * mouseStartDrag.Y)
                        / Math.Sqrt(MousePt.X * MousePt.X + MousePt.Y * MousePt.Y);

                    ThisTransformation.Scale = (float)len;
                    ThisTransformation.Pan = new Vector3f(0, 0, 0);
                    ThisTransformation.Rotation = new Quat4f();
                    ThisTransformation.MatrixMultiply(ThisTransformation, LastTransformation);// Accumulate Last Rotation Into This One
                }
                else if (isRightDrag) //pan
                {
                	float factor = 100.0f;	//this was added by ian
                	
                    float x = (float)(MousePt.X - mouseStartDrag.X) / (float)this.glControl1.Width;
                    float y = (float)(MousePt.Y - mouseStartDrag.Y) / (float)this.glControl1.Height;
                    float z = 0.0f;

//                  ThisTransformation.Pan = new Vector3f(-x*factor, -y*factor, z*factor);
                    ThisTransformation.Pan = new Vector3f(x*factor, -y*factor, z*factor);
                    ThisTransformation.Scale = 1.0f;
                    ThisTransformation.Rotation = new Quat4f();
                    ThisTransformation.MatrixMultiply(ThisTransformation, LastTransformation);
                }
                else if (isLeftDrag) //rotate
                {
                    ThisTransformation.Pan = new Vector3f(0, 0, 0);
                    ThisTransformation.Scale = 1.0f;
                    ThisTransformation.Rotation = ThisQuat;
                    ThisTransformation.MatrixMultiply(ThisTransformation, LastTransformation);
                }
            }
        }

        public void glOnMouseMove(object sender, MouseEventArgs e)
        {
            Point tempAux = new Point(e.X, e.Y);

            this.drag(tempAux);
            this.PlotGL();
         
        }

        /// <summary>
        /// start rotation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void glOnLeftMouseDown(object sender, MouseEventArgs e)
        {
            isLeftDrag = true;
            mouseStartDrag = new Point(e.X, e.Y);
            this.startDrag(mouseStartDrag);
//            this.PlotGL();
        }

        /// <summary>
        /// end rotation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void glOnLeftMouseUp(object sender, MouseEventArgs e)
        {
            isLeftDrag = false;
        }

        /// <summary>
        /// start pan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glOnRightMouseDown(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.SizeAll; 
            isRightDrag = true;
            mouseStartDrag = new Point(e.X, e.Y);
            this.startDrag(mouseStartDrag);
//            this.PlotGL();
        }

        /// <summary>
        /// end pan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glOnRightMouseUp(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Default;
            isRightDrag = false;
        }

        /// <summary>
        /// start zoom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glOnMiddleMouseDown(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.NoMove2D;
            isMiddleDrag = true;
            mouseStartDrag = new Point(e.X, e.Y);
            this.startDrag(mouseStartDrag);
//            this.PlotGL();
        }

        /// <summary>
        /// end zoom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glOnMiddleMouseUp(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Default;

            isMiddleDrag = false;
        }

        #endregion mouse control
		
        void DrawBackground()
        {
        	GL.Disable(EnableCap.Lighting);
        	GL.Disable(EnableCap.Blend);
        	GL.Disable(EnableCap.DepthTest);
        	GL.Disable(EnableCap.ClipPlane0);
        	
			GL.PushMatrix();
			
			// switch to projection mode
			GL.MatrixMode(MatrixMode.Projection);
			// save previous matrix which contains the
			//settings for the perspective projection
			GL.PushMatrix();
			// reset matrix
			GL.LoadIdentity();
			
			// set a 2D orthographic projection
			Glu.Ortho2D(0, glControl1.Width, 0, glControl1.Height);
//			// invert the y axis, down is positive
			GL.Scale(1, -1, 1);
			// mover the origin from the bottom left corner
			// to the upper left corner
			
			GL.Translate(0, -glControl1.Height, 0);
			GL.MatrixMode(MatrixMode.Modelview);
			
			GL.PushMatrix();
			GL.LoadIdentity();
			
			GL.PushAttrib(AttribMask.CurrentBit);
			
			GL.PolygonMode(MaterialFace.FrontAndBack,PolygonMode.Fill);
			GL.ShadeModel(ShadingModel.Smooth);
	
			GL.Begin(BeginMode.Quads);
				GL.Color3(0.5,0.7,0.9);
					GL.Vertex2(0,0);
					GL.Vertex2(glControl1.Width,0);
				GL.Color3(0.95,0.95,0.95);
					GL.Vertex2(glControl1.Width, glControl1.Height);
					GL.Vertex2(0,glControl1.Height);
			GL.End();

			GL.MatrixMode(MatrixMode.Projection);
			GL.PopMatrix();
			GL.MatrixMode(MatrixMode.Modelview);
			GL.PopMatrix();
			
			GL.PopAttrib();
			
			GL.Enable(EnableCap.Lighting);	//turn the lights back on
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.ClipPlane0);
        }
        
        void DrawGrid()
        {
        	GL.PushMatrix();
			GL.MultMatrix(matrix);
//			GL.Translate(0.0f-sceneCenter.X, 0.0f-sceneCenter.Y, 0.0f-sceneCenter.Z);
			GL.PushAttrib(AttribMask.CurrentBit);
			
			GL.Disable(EnableCap.Lighting);
			GL.Disable(EnableCap.ClipPlane0);
//			GL.Color3(Color.LightGray);
			GL.LineWidth(0.1f);
			GL.Begin(BeginMode.Lines);
			
			//draw the x lines
			for(int x=-50; x<=50; x++)
			{
				if(x%10 == 0)GL.Color3(Color.DarkGray);
				else GL.Color3(Color.LightGray);
				GL.Vertex3(x, 0.0, -50.0);
				GL.Vertex3(x, 0.0, 50.0);

			}
			for(int z=-50; z<=50; z++)
			{
				if(z%10 == 0)GL.Color3(Color.DarkGray);
				else GL.Color3(Color.LightGray);
				GL.Vertex3(-50.0, 0.0, z);
				GL.Vertex3(50.0, 0.0, z);

			}
			GL.End();
			GL.Enable(EnableCap.ClipPlane0);
			GL.Enable(EnableCap.Lighting);
			GL.PopAttrib();
			GL.PopMatrix();
        }
        
        void DrawAxes()
        {
        	GL.PushMatrix();
			GL.MultMatrix(matrix);
			GL.PushAttrib(AttribMask.CurrentBit);			
			GL.Disable(EnableCap.Lighting);
			GL.Disable(EnableCap.ClipPlane0);
			GL.LineWidth(3.0f);
			GL.Begin(BeginMode.Lines);
			
			//x line
			GL.Color3(1.0f,0.0f,0.0f);
			GL.Vertex3(0.0,0.0,0.0);
			GL.Vertex3(5.0,0.0,0.0);
			//y line
			GL.Color3(0.0f,1.0f,0.0f);
			GL.Vertex3(0.0,0.0,0.0);
			GL.Vertex3(0.0,5.0,0.0);
			
			//z line
			GL.Color3(0.0f,0.0f,1.0f);
			GL.Vertex3(0.0,0.0,0.0);
			GL.Vertex3(0.0,0.0,5.0);
			
			GL.End();
			
			GL.Enable(EnableCap.ClipPlane0);
			GL.Enable(EnableCap.Lighting);
			GL.PopAttrib();
			GL.PopMatrix();
        }
        
        void DrawBoundingBoxes()
        {
        	//TODO:Draw bounding boxes	
        }
        
		void Load_buttClick(object sender, EventArgs e)
		{
			OpenFileDialog of = new OpenFileDialog();
			of.Filter = "Xml Files|*.xml|OBJ Files|*.obj";
			
			if (of.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			else
			{
				this.xmlFileName_tb.Text = of.FileName;
			}
			
			_bhxml.Reset();		//reset the xml parser
			if(of.FileName.EndsWith(".xml"))
			{
				_bhxml.LoadXml(of.FileName);
				_bhxml.ReadFromXML(_bhxml.XmlDoc, ref _nodeList, ref _barList);	//, ref _plateList);
			}
			else if(of.FileName.EndsWith(".obj"))
			{
				ikeo.OBJReader objRead = new OBJReader(of.FileName);
				_meshList = objRead.meshes;

				MessageBox.Show(_meshList.Count + " meshes loaded...");
			}
			   
//			SetupVertexArray();	
			SetupMeshVertexArrays();	//don't need to do this with new mesh reader
			
			SetupClipPlanes();

//			RebuildVertexArray();
			
			_modelLoaded = true;

			//try loading the image texture
			//try first with the model file path +.png
			System.IO.FileInfo fi = new System.IO.FileInfo(of.FileName);
			_texturePath = of.FileName.TrimEnd(fi.Extension.ToCharArray()) + ".png";
			
			if (LoadTexture()) 
			{
                UploadTexture(); 
				
			}
            else
            {
//                throw new System.IO.FileNotFoundException(_texturePath);
            	_imageLoaded = false;
            }
            
            _imageLoaded = true;
		}
	
		void DrawClipQuads()
		{
			GL.Disable(EnableCap.Lighting);
			GL.Color3(Color.Orange);
			GL.Disable(EnableCap.CullFace);	//turn off face culling for the clip plane
			GL.PushMatrix();
			GL.MultMatrix(matrix);
			GL.Translate(0.0f-sceneCenter.X, 0.0f-sceneCenter.Y, 0.0f-sceneCenter.Z);
			GL.Begin(BeginMode.Quads);
			for(int i=3; i>=0; i--)
			{
				GL.Vertex3(_cl0.quad[i]);
			}
			GL.End();
			GL.PopMatrix();
			GL.Enable(EnableCap.CullFace);	//renable culling on the clip plane
			GL.Enable(EnableCap.Lighting);
		}
	
		void DrawClipPlanes()
		{

			GL.PushMatrix();
			GL.MultMatrix(matrix);
			GL.ClipPlane(ClipPlaneName.ClipPlane0, new double[]{_cl0.norm.X, _cl0.norm.Y, _cl0.norm.Z});
			GL.PopMatrix();
		}
	
		private bool LoadTexture()
        {
            if (!System.IO.File.Exists(_texturePath))
                return false;

            _bitmap = new Bitmap(_texturePath);
            return true;
        }
		
		private void UploadTexture()
        {
            _texture = GL.GenTexture();
           	GL.BindTexture(TextureTarget.Texture2D, _texture);
           	
            BitmapData data = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
       
           	GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvModeCombine.Replace);
//            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToBorder);
//            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToBorder);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            _bitmap.UnlockBits(data);

            

        }
	}
	
	#region MouseControl
    public class MouseControl
    {
        protected Control newCtrl;
        protected MouseButtons FinalClick;

        public event EventHandler LeftClick;
        public event EventHandler RightClick;
//        public event EventHandler MiddleClick;

        public event MouseEventHandler LeftMouseDown;
        public event MouseEventHandler LeftMouseUp;
        public event MouseEventHandler RightMouseDown;
        public event MouseEventHandler RightMouseUp;
 
        public event MouseEventHandler MiddleMouseDown;
        public event MouseEventHandler MiddleMouseUp;

        public Control Control
        {
            get { return newCtrl; }
            set
            {
                newCtrl = value;
                Initialize();
            }
        }

        public MouseControl()
        {
        }

        public MouseControl(Control ctrl)
        {
            Control = ctrl;
        }

        public void AddControl(Control ctrl)
        {
            Control = ctrl;
        }

        protected virtual void Initialize()
        {
            newCtrl.Click += new EventHandler(OnClick);
            newCtrl.MouseDown += new MouseEventHandler(OnMouseDown);
            newCtrl.MouseUp += new MouseEventHandler(OnMouseUp);

        }

        private void OnClick(object sender, EventArgs e)
        {
            switch (FinalClick)
            {
                case MouseButtons.Left:
                    if (LeftClick != null)
                    {
                        LeftClick(sender, e);
                    }
                    break;

                case MouseButtons.Right:
                    if (RightClick != null)
                    {
                        RightClick(sender, e);
                    }
                    break;
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            FinalClick = e.Button;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        if (LeftMouseDown != null)
                        {
                            LeftMouseDown(sender, e);
                        }
                        break;
                    }
                case MouseButtons.Middle:
                    {
                        if (MiddleMouseDown != null)
                        {
                            MiddleMouseDown(sender, e);
                        }
                        break;
                    }

                case MouseButtons.Right:
                    {
                        if (RightMouseDown != null)
                        {
                            RightMouseDown(sender, e);
                        }
                        break;
                    }
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        if (LeftMouseUp != null)
                        {
                            LeftMouseUp(sender, e);
                        }
                        break;
                    }
                case MouseButtons.Middle:
                    {
                        if (MiddleMouseUp != null)
                        {
                            MiddleMouseUp(sender, e);
                        }
                        break;
                    }

                case MouseButtons.Right:
                    {
                        if (RightMouseUp != null)
                        {
                            RightMouseUp(sender, e);
                        }
                        break;
                    }
            }
        }
    }
    #endregion MouseControl

	public class ClipPlane
	{
		public Vector3[] quad;
		public Vector3 norm;
		
		public ClipPlane(Vector3 v1, Vector3 v2, Vector3 center)
		{
			norm = Vector3.Cross(v1, v2);	//find the normal of the clip plane

			quad = new Vector3[4];
			
			float l = 1000.0f;
			
			v1 = new Vector3(v1.X*l, v1.Y*l, v1.Z*l);
			v2 = new Vector3(v2.X*l, v2.Y*l, v2.Z*l);
			
			quad[0] = center + v1 + v2;
			quad[1] = center - v1 + v2;
			quad[2] = center - v1 - v2;
			quad[3] = center + v1 - v2;

		}
	}

	class CustomGLControl : OpenTK.GLControl
{
    public CustomGLControl(): base(new GraphicsMode(32, 24, 8))
    { }
}

}
