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
using System.Drawing;

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
		bool loaded = false;	//check for form loading
		
		#region file loading and updating
		BuroHappold.Interface.BHXml_Parser _bhxml = new BHXml_Parser();
		bool _loaded = false;
		#endregion
		
		#region text
		TextureFont _sans = new TextureFont(new Font(FontFamily.GenericSansSerif, 12.0f));
		ITextPrinter _text = new TextPrinter();
		
		#endregion
		
		#region animation
		float rotation = 0;
		Stopwatch sw = new Stopwatch();
		int idleCounter = 0;
		double accumulator = 0;
		#endregion
		
		#region drawing settings
		float _lineSize = 1.0f;		//gl line render size
		float _pointSize = 7.0f;		//gl node render size
		float _red = 0.0f;			//gl red color
		float _green = 0.0f;			//gl green color
		float _blue = 0.0f;			//gl blue color;
//		float _alpha = 1.0f;			//gl alpha color;
		
		#endregion
		
		#region global object/display lists
		
		List<BHBar> _barList 		= new List<BHBar>();		//global list for all bars and lines
		List<BHNode> _nodeList 		= new List<BHNode>();		//global list for all nodes
		List<BHPlate> _plateList 	= new List<BHPlate>();		//global list for all plates
		List<Mesh> _meshList		= new List<Mesh>();			//global list for all meshes
		List<float[]> _meshVerts	= new List<float[]>();		//list for mesh vertex arrays
		List<int[]> _meshVertInd	= new List<int[]>();		//list for mesh vertex index arrays
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
        
       	#region lighting
       	private static float[] _LightAmbient = new float[]{0.1f,0.1f,0.1f,1.0f};
       	private static float[] _LightDiffuse = new float[]{1.0f,1.0f,1.0f,1.0f};
       	private static float[] _LightPosition = new float[]{0.0f,0.0f,-100.0f,1.0f};
       	private static float[] _LightSpecular = new float[]{1.0f,1.0f,1.0f,1.0f};

       	private static float[] _MaterialSpecular = new float[]{0.0f,0.0f,0.0f,1.0f};
       	private static float[] _SurfaceShininess = new float[]{0.0f,0.0f,0.0f,1.0f};
       	private static float[] _MaterialDiffuse = new float[]{1.0f,0.0f,0.0f,1.0f};
       	
		#endregion
      	
        #region mouse
        private Point mouseStartDrag;
        private static bool isLeftDrag = false;
        private static bool isRightDrag = false;
        private static bool isMiddleDrag = false;
		#endregion
		
		#region matrix info setup
		double[] _modelViewMatrix = new double[16];
		double[] _projectionMatrix = new double[16];
		int[] _viewport = new int[4];
	    Vector3 _objPoint 				= new Vector3(0.0f, 0.0f, 0.0f);
	    Vector3 _winPoint 				= new Vector3(0f,0f,0f);
	    Vector3 _worldPoint 			= new Vector3(0f,0f,0f);
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
			loaded = true;							//make sure the form is loaded
			
			this.WindowState = FormWindowState.Maximized;	//maximize the window
			
			glControl1.SwapBuffers();

			glControl1.Load += new EventHandler(GlControlLoad);
			glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(glOnMouseMove);
			glControl1.SizeChanged += new EventHandler(GlControl1Resize);
			
			GlControl1Resize(sender, e);
			
			//arcball
			LastTransformation.SetIdentity(); // Reset Rotation
            ThisTransformation.SetIdentity(); // Reset Rotation
            ThisTransformation.get_Renamed(matrix);
            
            Application.Idle +=	new EventHandler(Application_Idle);

//			torus1(0.2f, 0.5f); // load some graphs from a display list

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
				label1.Text = idleCounter.ToString();
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
			if (!loaded)	//check for loading
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
			this.viewport1.Location = new Point(200, 25);
			this.viewport1.Width = Size.Width - 200;
			this.viewport1.Height = Size.Height - 25;
			
            GL.ShadeModel(ShadingModel.Flat);								// enable smooth shading
            GL.ClearColor(Color.LightGray);									// black background
            GL.ClearDepth(1.0f);											// depth buffer setup
            GL.Enable(EnableCap.DepthTest);									// enables depth testing
            GL.Enable(EnableCap.PointSmooth);								// point smoothing
            GL.Enable(EnableCap.LineSmooth);
//			GL.DepthFunc(DepthFunction.Greater);								// type of depth testing
            
			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);	// nice perspective calculations

            GL.Viewport(0, 0,
                        viewport1.Width, 
                        viewport1.Height);
            
            GL.PushMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Glu.Perspective(60.0, (double)this.glControl1.Width / (double)this.glControl1.Height, 1.0, 10000.0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
            
            #region lighting
            GL.Enable(EnableCap.Lighting);                                      // Enable Lighting
			GL.Enable(EnableCap.Light0);
            
			GL.Light(LightName.Light0, LightParameter.Diffuse, _LightDiffuse);
            GL.Light(LightName.Light0, LightParameter.Position, _LightPosition);
            GL.Light(LightName.Light0, LightParameter.Specular, _LightSpecular);

            GL.Enable(EnableCap.ColorMaterial);	 // Enable Color Material
            GL.ColorMaterial(MaterialFace.Front,ColorMaterialParameter.AmbientAndDiffuse);
            
//            GL.FrontFace(FrontFaceDirection.Ccw);
//            
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, _MaterialSpecular);
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, _SurfaceShininess);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, _MaterialDiffuse);

            #endregion lighting

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
		/// Generate the display list for the torus
		/// </summary>
		/// <param name="MinorRadius"></param>
		/// <param name="MajorRadius"></param>
		private void torus1(float MinorRadius, float MajorRadius)                    // Draw A Torus With Normals
        {
            int i, j;
            int stacks = 100;
            int slices = 100;
            _faces = GL.GenLists(1);						    	// build a display list
            GL.NewList(_faces, ListMode.Compile);					// start loading into diplay list
            GL.Begin(BeginMode.TriangleStrip);                      // Start A Triangle Strip
            for (i = 0; i < stacks; i++)                            // Stacks
            {
                for (j = -1; j < slices; j++)                       // Slices
                {
                    float wrapFrac = (j % stacks) / (float)slices;
                    double phi = Math.PI * 2.0 * wrapFrac;
                    float sinphi = (float)(Math.Sin(phi));
                    float cosphi = (float)(Math.Cos(phi));

                    float r = MajorRadius + MinorRadius * cosphi;

                    GL.Normal3(
                            (Math.Sin(Math.PI * 2.0 * (i % stacks + wrapFrac) / (float)slices)) * cosphi,
                            sinphi,
                            (Math.Cos(Math.PI * 2.0 * (i % stacks + wrapFrac) / (float)slices)) * cosphi);
                    GL.Vertex3(
                            (Math.Sin(Math.PI * 2.0 * (i % stacks + wrapFrac) / (float)slices)) * r,
                            MinorRadius * sinphi,
                            (Math.Cos(Math.PI * 2.0 * (i % stacks + wrapFrac) / (float)slices)) * r);

                    GL.Normal3(
                            (Math.Sin(Math.PI * 2.0 * (i + 1 % stacks + wrapFrac) / (float)slices)) * cosphi,
                            sinphi,
                            (Math.Cos(Math.PI * 2.0 * (i + 1 % stacks + wrapFrac) / (float)slices)) * cosphi);
                    GL.Vertex3(
                            (Math.Sin(Math.PI * 2.0 * (i + 1 % stacks + wrapFrac) / (float)slices)) * r,
                            MinorRadius * sinphi,
                            (Math.Cos(Math.PI * 2.0 * (i + 1 % stacks + wrapFrac) / (float)slices)) * r);
                }
            }
            GL.End();                                                        // Done Torus
            GL.EndList(); // end dispaly list
        }

		/// <summary>
		/// Looks at all user input and sets values
		/// </summary>
		private void PollUserInput()
		{
			
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
				int[]ind		= new int[m.faceCount()*3];
				float[]text		= new float[m.vertexCount()*2];
				float[]norms	= new float[m.vertexCount()*3];
				
				int vertCount = 0;
				int textCount = 0;
				int indCount = 0;
				int normCount = 0;
				
				//fill up the verts array
				for(int i=0; i<m.vertexCount(); i++)
				{
					verts[vertCount] = (float)m.vertices[i].v.x;	// - sceneCenter.X;
					verts[vertCount+1] = (float)m.vertices[i].v.y;	// - sceneCenter.Y;
					verts[vertCount+2] = (float)m.vertices[i].v.z;	// - sceneCenter.Z;
					vertCount += 3;
					
					norms[normCount] = (float)m.vertices[i].normal.x;
					norms[normCount+1] = (float)m.vertices[i].normal.y;
					norms[normCount+2] = (float)m.vertices[i].normal.z;
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
					text[textCount] = (float)m.vertices[i].tex.x;
					text[textCount + 1] = (float)m.vertices[i].tex.y;
					textCount += 2;
				}
				
				//fill up the faces index array
				for(int i=0; i<m.faceCount(); i++)
				{
					ind[indCount] = m.faces[i].vertices[0].label;
					ind[indCount+1] = m.faces[i].vertices[1].label;
					ind[indCount+2] = m.faces[i].vertices[2].label;
					
					indCount += 3;
				}
				
				//add all the arrays to the lists
				_meshVerts.Add(verts);
				_meshVertInd.Add(ind);
				_textCoords.Add(text);
				_meshNormals.Add(norms);
				
			}
			
			sceneCenter.X = avgX/vCount;
			sceneCenter.Y = avgY/vCount;
			sceneCenter.Z = avgZ/vCount;
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
			
			Glu.LookAt(0.0f, 0.0f, -100.0f, sceneCenter.X, sceneCenter.Z, sceneCenter.Y, 0.0, 1.0, 0.0);
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
            
			GL.Color3(Color.Black);	//draw black points
            GL.PointSize(_pointSize);
            GL.EnableClientState(EnableCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 0, _points);
            GL.DrawElements(BeginMode.Points, _pointInd.Length, DrawElementsType.UnsignedInt, _pointInd);
           	GL.DisableClientState(EnableCap.VertexArray);	
           	
           	GL.PopMatrix(); // NEW: Unapply Dynamic Transform
		}
		
		private void DrawMeshes()
		{
			

			GL.PushMatrix();
			GL.MultMatrix(matrix);           // NEW: Apply Dynamic Transform
			GL.Translate(-sceneCenter.X, -sceneCenter.Y, -sceneCenter.Z);	//move the model to 0,0,0
			GL.Enable(EnableCap.CullFace);	
			
			GL.EnableClientState(EnableCap.VertexArray);
			GL.EnableClientState(EnableCap.NormalArray);
				
			for(int i=0; i<_meshVerts.Count; i++)	//pick one of the lists to use as the count
			{

				float[] v 	= _meshVerts[i];
				float[] t 	= _textCoords[i];
				int[] ind	= _meshVertInd[i];
				float[] n	= _meshNormals[i];
				
				GL.NormalPointer(NormalPointerType.Float, 0, n);
				GL.VertexPointer(3, VertexPointerType.Float, 0, v);
				
				//DRAW FACES
				GL.Color3(Color.BlanchedAlmond);
				GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
				GL.DrawElements(BeginMode.Triangles,ind.Length, DrawElementsType.UnsignedInt, ind);
				
				//TURN OFF LIGHTS FOR POINTS AND LINES
				GL.Disable(EnableCap.Lighting);
				
				//DRAW WIREFRAME
				GL.LineWidth(_lineSize);
				GL.Color3(Color.Black);
				GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
				GL.DrawElements(BeginMode.Triangles,ind.Length, DrawElementsType.UnsignedInt, ind);

				//DRAW POINTS
//				GL.PointSize(_pointSize);
//				GL.Color3(Color.Red);
//				GL.PolygonMode(MaterialFace.Front, PolygonMode.Point);
//				GL.DrawElements(BeginMode.Triangles,ind.Length, DrawElementsType.UnsignedInt, ind);
				
				//TURN LIGHTS BACK ON
				GL.Enable(EnableCap.Lighting);

			}
			
			GL.DisableClientState(EnableCap.NormalArray);
			GL.DisableClientState(EnableCap.VertexArray);
			
			GL.Disable(EnableCap.CullFace);	
			GL.PopMatrix();
		}
		
		/// <summary>
		/// Draw node numbers
		/// </summary>
		private void DrawNodeNumbers()
		{

            GL.PushMatrix();                 // NEW: Prepare Dynamic Transform
            GL.MultMatrix(matrix);           // NEW: Apply Dynamic Transform
            
           	foreach (BHNode n in _nodeList)
              {

	              double[] modelViewMatrix = new double[16];
	              double[] projectionMatrix = new double[16];
	              int[] viewport = new int[4];
	               
	              Vector3 objPoint = new Vector3((float)n.X, 
	                                              (float)n.Z,
	                                              (float)n.Y);
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

            			GL.Color3(Color.PaleGoldenrod);
            			//translate has a weird winY that needs to be further translated by viewport[3]
            			GL.Translate(winPoint.X, viewport[3] - winPoint.Y, 0.0f);
           		 		_text.Draw(n.Id.ToString(), _sans);

            		_text.End();

               }
			GL.PopMatrix(); // NEW: Unapply Dynamic Transform
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

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // Clear screen and DepthBuffer

                GL.LoadIdentity();

				DrawBackground();
//				DrawGrid();
				
				_LightPosition = new float[]{(float)sceneCenter.X, (float)sceneCenter.Y + 1000.0f,(float)sceneCenter.Z,1.0f};
				GL.Light(LightName.Light0, LightParameter.Position, _LightPosition);
				
              	//use an initial translate to move the model along the Z axis
              	//this is like backing the viewpoint away from the scene.
              	//the camera is always at 0,0,0
              	
              	//look at the scene center
              	//TODO: back this off by the distance
              	//of the scene bounding sphere
				Glu.LookAt(0.0, 0.0, -100.0,
              	           0.0,0.0,0.0,
              	           0.0, 1.0, 0.0);
              	
//				Glu.LookAt(0.0, -1.0, 0.0,
//              	           0.0,0.0,0.0,
//              	           0.0, 1.0, 0.0);
              	
                #region plot all diplay lists
                
                if (_loaded)
                {
                	//calc distance to scene center
                	double dist	= 	Math.Sqrt(Math.Pow(sceneCenter.X,2) + 
              						Math.Pow(sceneCenter.Y,2) + 
              						Math.Pow(sceneCenter.Z,2));
                	this.pointLabel.Text = "DTC : " + dist;
                	
//              		GL.Translate(0.0,0.0,-dist);
//              	GL.Translate(0.0,0.0,0.0);
              	
	                //draw our points, lines, and faces
//	                DrawLines();			//draw the lines
//	              	DrawPoints();			//draw the points
	              	DrawMeshes();
//					DrawNodeNumbers();
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

                  ThisTransformation.Pan = new Vector3f(-x*factor, -y*factor, z*factor);
//                    ThisTransformation.Pan = new Vector3f(x, y, z);
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
			
			GL.ShadeModel(ShadingModel.Smooth);
	
			GL.Begin(BeginMode.Quads);
				GL.Color3(0.4,0.4,0.4);
					GL.Vertex2(0,0);
					GL.Vertex2(glControl1.Width,0);
				GL.Color3(0.75,0.75,0.75);
					GL.Vertex2(glControl1.Width, glControl1.Height);
					GL.Vertex2(0,glControl1.Height);
			GL.End();

			GL.MatrixMode(MatrixMode.Projection);
			GL.PopMatrix();
			GL.MatrixMode(MatrixMode.Modelview);
			GL.PopMatrix();
			
			GL.Enable(EnableCap.Lighting);	//turn the lights back on
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.DepthTest);
        }
        
        void DrawGrid()
        {
        	//TODO://implement grid drawing
        }
		
        void LineWidth_trackBarScroll(object sender, EventArgs e)
		{
			_lineSize = (float)this.lineWidth_trackBar.Value;
		}
		
		void Blue_sliderScroll(object sender, EventArgs e)
		{
			_blue = this.blue_slider.Value * .1f;
		}
		
		void Green_sliderScroll(object sender, EventArgs e)
		{
			_green = this.green_slider.Value * .1f;
		}
		
		void Red_sliderScroll(object sender, EventArgs e)
		{
			_red = this.red_slider.Value * .1f;
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
				_meshList = ikeo.OBJReader.LoadMeshesFromOBJ(of.FileName);
				MessageBox.Show(_meshList.Count + " meshes loaded...");
			}
			   
			SetupVertexArray();
			SetupMeshVertexArrays();
			
//			RebuildVertexArray();
			
			_loaded = true;
			
//			ZoomToFill();
			
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
}
