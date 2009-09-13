/*
 * Created by SharpDevelop.
 * User: rrosenma
 * Date: 7/15/2008
 * Time: 5:16 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using BuroHappold.Analysis.Elements;
using BuroHappold.Analysis.Tensyl;
using BuroHappold.MathUtil;

namespace BuroHappold.Analysis.Elements
{
	/// <summary>
	/// Element class
	/// The top level class.
	/// All Buro Happold analysis objects inherit from this class.
	/// </summary>
	public class BHElement
	{
		#region private members
		private int m_id;
		#endregion
		
		#region properties
		public int Id
		{
			get
			{
				return m_id;
			}
			set
			{
				m_id = value;
			}
		}
		#endregion
		
		public BHElement(int id)
		{
			m_id = id;
		}
	}
	
	/// <summary>
	/// Node Class
	/// Inherits from BHElement
	/// Used for describing a point in space with analytical properties.
	/// </summary>
	public class BHNode : BHElement, IComparable
	{
		#region private members
		private double 		m_x;
		private double 		m_y;
		private double 		m_z;
		private int 		m_fixity;
		private double 		m_stiffness;
		private double 		m_resForceX;
		private double 		m_resForceY;
		private double 		m_resForceZ;
		private double 		m_i;
		private double 		m_j;
		private BHMesh		m_mesh;
		private Vector3 	m_xyz;
		private Vector3		m_prevXYZ;
		private Vector3 	m_load;
		private BHAxSym		m_axSym = null;
		private bool 		m_fixed;
		private double 		m_Ri;
		private double		m_selfWeight;
		private List<BHNode> m_connectedNodes;
		private Vector3		m_appliedLoad;
		
		private static SortMethod sortOrder;
		
		private static double distanceTolerance = 0.01;	// used for checking if 2 BHNodes are equivalent
		
		#endregion
		
		/// <summary>
		/// the enumerator for the sort method
		/// </summary>
		public enum SortMethod
		{idSort = 0, xSort = 1, ySort = 2, zSort = 3};
		
		#region properties
		
		public double ResidualForceX
		{
			get
			{
				return m_resForceX;
			}
			set
			{
				m_resForceX = value;
			}
		}
		public double ResidualForceY
		{
			get
			{
				return m_resForceY;
			}
			set
			{
				m_resForceY = value;
			}
		}
		public double ResidualForceZ
		{
			get
			{
				return m_resForceZ;
			}
			set
			{
				m_resForceZ = value;
			}
		}
		public Vector3 AppliedLoad
		{
			get
			{
				return m_appliedLoad;
			}
			set
			{
				m_appliedLoad = value;
			}
		}
		public double Stiffness
		{
			get
			{
				return m_stiffness;
			}
			set
			{
				m_stiffness = value;
			}
		}
		public double X
		{
			get
			{
				return m_x;
			}
			set
			{
				m_x = value;
			}
		}
		public double Y
		{
			get
			{
				return m_y;
			}
			set
			{
				m_y = value;
			}
		}
		public double Z
		{
			get
			{
				return m_z;
			}
			set
			{
				m_z = value;
			}
		}
		public double i
		{
			get
			{
				return m_i;
			}
			set
			{
				m_i = value;
			}
		}
		public double j
		{
			get
			{
				return m_j;
			}
			set
			{
				m_j = value;
			}
		}
		public Vector3 XYZ
		{
			get
			{
				return new Vector3((float)m_x, (float)m_y, (float)m_z);
			}
			
		}
		public Vector3 PreviousXYZ
		{
			get
			{
				return m_prevXYZ;
			}
			set
			{
				m_prevXYZ = value;
			}
		}
		public Vector3 Load
		{
			get
			{
				return m_load;
			}
			set
			{
				m_load = value;
			}
		}
		public BHAxSym AxSymConnection
		{
			get
			{
				return m_axSym;
			}
			set
			{
				m_axSym = value;
			}
		}
		public double Ri
		{
			get
			{
				Vector3 currentRi = new Vector3((float)this.ResidualForceX, (float)this.ResidualForceY, (float)this.ResidualForceZ);
				double totalRi = currentRi.VectorMag(currentRi);
				m_Ri = totalRi;
				
				return m_Ri;
			}
			
		}
		
		/// <summary>
		/// return the fixity of the BHNode
		/// fixity is coded as:
		///0--no fixity
		///1--x
		///2--y
		///3--z
		///4--xy
		///5--xz
		///6--yz
		///7--all fixed
		/// </summary>
		public int Fixity
		{
			get
			{
				return m_fixity;
			}
			set
			{
				m_fixity = value;
			}
		}
		public bool IsFixed
		{
			get
			{
				if (this.Fixity == 7)
				{
					m_fixed = true;
					return m_fixed;
				}
				else
				{
					m_fixed = false;
					return m_fixed;
				}
			}
		}
		public double SelfWeight
		{
			get
			{
				return m_selfWeight;
			}
			set
			{
				m_selfWeight = value;
			}
		}
		public List<BHNode> ConnectedNodes
		{
			get
			{
				return m_connectedNodes;
			}
			set
			{
				m_connectedNodes = value;
			}
		}
		
		public static SortMethod SortOrder {
			get { return sortOrder; }
			set { sortOrder = value; }
		}
		#endregion
		
		#region constructors
		
		/// <summary>
		/// Creates a BHNode object with a unique ID, x,y,z coordinates and i,j parameters in a mesh.
		/// </summary>
		/// <param name="id">unique identifier</param>
		/// <param name="x">x coordinate</param>
		/// <param name="y">y coordinate</param>
		/// <param name="z">z coordinate</param>
		/// <param name="i">mesh parameter</param>
		/// <param name="j">mesh parameter</param>
		/// <param name="m">the mesh</param>
		public BHNode(int id, double x, double y, double z, int i, int j, BHMesh m): base(id)
		{
			m_xyz = new Vector3((float)x, (float)y, (float)z);
			m_prevXYZ = new Vector3(0f, 0f, 0f);
			
			this.Id = id;
			
			m_x = x;
			m_y = y;
			m_z = z;
			
			m_resForceX = 0.0;
			m_resForceY = 0.0;
			m_resForceZ = 0.0;
			
			m_i = i;
			m_j = j;
			m_mesh = m;
		}
		
		/// <summary>
		/// Creates a BHNode based on a unique ID
		/// </summary>
		/// <param name="id"></param>
		public BHNode(int id): base(id)
		{
			//an empty constructor
		}
		
		/// <summary>
		/// Creates a BHNode based on position and unique ID
		/// </summary>
		/// <param name="id"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public BHNode(int id, double x, double y, double z) : base(id)
		{
			m_xyz = new Vector3((float)x, (float)y, (float)z);
			m_prevXYZ = new Vector3(0f, 0f, 0f);
			
			m_x = x;
			m_y = y;
			m_z = z;
			
			m_resForceX = 0.0;
			m_resForceY = 0.0;
			m_resForceZ = 0.0;
		}
		
		#endregion
		
		/// <summary>
		/// CompareTo method necessary when implementing the IComparable interface
		/// </summary>
		/// <param name="obj">object being compared to</param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if (obj is BHNode)
			{
				//define a locator as the comparee
				BHNode node = (BHNode) obj;
				
				//define a switch for different search types
				switch (sortOrder)
				{
					case SortMethod.idSort:
						return this.Id.CompareTo(node.Id);
					case SortMethod.xSort:
						return m_x.CompareTo(node.X);
					case SortMethod.ySort:
						return m_y.CompareTo(node.Y);
					case SortMethod.zSort:
						return m_z.CompareTo(node.Z);
				}
			}
			throw new ArgumentException("Object is not a BHNode");
		}
		
		// Set all the forces on the node to zero.
		public virtual void SetAllForcesToZero()
		{
			m_stiffness = 0.0;
			m_resForceX = 0.0;
			m_resForceY = 0.0;
			m_resForceZ = 0.0;
		}
		
		// Calculate the distance between this node and another.
		public virtual double Distance(BHNode b)
		{
			double distance = System.Math.Sqrt(System.Math.Pow((b.X - this.X),2) +
			                                   System.Math.Pow((b.Y - this.Y),2) +
			                                   System.Math.Pow((b.Z - this.Z),2));
			return distance;
		}
		
		public bool AlmostEqual(BHNode b)
		{
			if (this.Distance(b) < BHNode.distanceTolerance)
				return true;
			else
				return false;
		}
	}

	/// <summary>
	/// Bar class
	/// Inherits from BHElement
	/// Used for describing a linear element between two nodes.
	/// </summary>
	public class BHBar : BHElement
	{
		#region private members
		
		private BHNode m_start;
		private BHNode m_end;
		private double m_length;
		private double m_slackLength;
		private double m_nominalLength;

		private double m_ea;
		private double m_force;
		private double m_xDirection;
		private double m_yDirection;
		private double m_zDirection;
		private double m_memberTension;
		private Vector3 m_direction;
		private BHCProp m_prop;				//the cable prop -- for Tensyl
		private int m_control;				//0--elastic, 1--force
		private Color m_color;

		#endregion
		
		#region properties
		
		//!*The start node of the bar.*!
		public BHNode StartNode
		{
			get
			{
				return m_start;
			}
		}
		
		//!*The end node of the bar.*!
		public BHNode EndNode
		{
			get
			{
				return m_end;
			}
		}
		
		//!*The length of the bar.*!
		public double Length
		{
			get
			{
				double xDist = (m_end.X - m_start.X);
				double yDist = (m_end.Y - m_start.Y);
				double zDist = (m_end.Z - m_start.Z);
				
				m_length = System.Math.Sqrt(System.Math.Pow(xDist, 2) +
				                            System.Math.Pow(yDist, 2) +
				                            System.Math.Pow(zDist, 2));
				
				return m_length;
			}
		}
		
		//!*The slack length of the bar.*!
		public double SlackLength
		{
			get
			{
				return m_slackLength;
			}
			set
			{
				m_slackLength = value;
			}
		}
		
		//!*The stiffness of the bar.*!
		public double EA
		{
			get
			{
				return m_ea;
			}
			set
			{
				m_ea = value;
			}
		}
		
		//!*The x component of the bar direction.*!
		public double XDirection
		{
			get
			{
				m_xDirection = m_end.X - m_start.X;
				return m_xDirection;
			}
		}
		
		//!*The y component of the bar direction.*!
		public double YDirection
		{
			get
			{
				m_yDirection = m_end.Y - m_start.Y;
				return m_yDirection;
			}
		}
		
		//!*The z component of the bar direction.*!
		public double ZDirection
		{
			get
			{
				m_zDirection = m_end.Z - m_start.Z;
				return m_zDirection;
			}
		}
		
		//!*The vector of the bar direction.*!
		public Vector3 Direction
		{
			get
			{
				Vector3 startXYZ = this.StartNode.XYZ;
				Vector3 endXYZ = this.EndNode.XYZ;
				
				Vector3 direction = endXYZ.Direction(startXYZ);	//subtract the start and end to get a direction
				
				m_direction = direction;
				
				return m_direction;
			}
		}
		
		//!*The axial force in the bar.*!
		public double AxialForce
		{
			get
			{
				return m_force;
			}
			set
			{
				m_force = value;
			}
		}
		
		//!*The tension in the bar.*!
		public double MemberTension
		{
			get
			{
				return m_memberTension;
			}
		}
		
		//!*The cable property of the bar. Used for Tensyl analysis.*!
		public BHCProp CPropId
		{
			get
			{
				return m_prop;
			}
			set
			{
				m_prop = value;
			}
		}
		
		//!*The control type of the bar. 0-Force, 1-Elastic. Used for Tensyl analysis.*!
		public int Control
		{
			get
			{
				return m_control;
			}
			set
			{
				m_control = value;
			}
		}
		
		//!*The color of the bar.*!
		public Color Color
		{
			get
			{
				return m_color;
			}
			set
			{
				m_color = value;
			}
		}
		#endregion
		
		#region methods
		//!Constructor to define a BHBar object by two points and some analtical properties.
		public BHBar(int id, ref BHNode a, ref BHNode b, double slackLength, BHCProp prop, int control) : base(id)
		{
			m_start = a;
			m_end = b;

			m_force = 0.0;
			
			m_prop = prop;
			m_control = control;
			
			m_nominalLength = a.Distance(b);	//set the nominal length

			m_slackLength = this.Length;		//set the slackLength;
		}
		
		//!Constructor to define a BHBar object by two points. *** RR: is this ever used? ***
		public BHBar(int id, BHNode a , BHNode b):base(id)
		{
			m_start = a;
			m_end = b;
		}
		
		public BHBar(int id, BHNode a , BHNode b, BHCProp prop):base(id)
		{
			m_start = a;
			m_end = b;
			m_slackLength = this.Length;
			m_control = 1;
			m_prop = prop;
		}
		
		//!Calculate the tension in the member.
		public void CalculateMemberTension(double prestress)//, int deadLoadOnly)
		{
			//TODO:Write in code for compression vs. tension members -- see joe
			//if the member is in compression, zero out the value
			//it will not contribute in this step of the solution.

			double LDiff = this.Length - m_slackLength;

//			if (LDiff <= 0)		//if the member is in compression
//			{
//				m_memberTension = 0.0;
//			}
//			else
//			{
			m_memberTension = prestress + this.EA * LDiff/m_slackLength;
//			}

		}
		
		//!Calculate the residual force in the member.
		public void CalculateResidualForce()
		{
			//TODO: check this method for small angle theory
			this.StartNode.ResidualForceX += this.MemberTension * (this.XDirection/m_slackLength);
			this.StartNode.ResidualForceY += this.MemberTension * (this.YDirection/m_slackLength);
			this.StartNode.ResidualForceZ += this.MemberTension * (this.ZDirection/m_slackLength);
			
			this.EndNode.ResidualForceX -= this.MemberTension * (this.XDirection/m_slackLength);
			this.EndNode.ResidualForceY -= this.MemberTension * (this.YDirection/m_slackLength);
			this.EndNode.ResidualForceZ -= this.MemberTension * (this.ZDirection/m_slackLength);
		}
		
		//!Calculate the effective stiffness of the member.
		public void CalculateEffectiveStiffness()
		{

			double tempEA = (this.EA)/m_slackLength;
			
			m_start.Stiffness 	+= tempEA + (this.MemberTension/this.Length);
			m_end.Stiffness		+= tempEA + (this.MemberTension/this.Length);

		}
		
		/// <summary>
		/// Draw bar to openGL context
		/// </summary>
		public void Draw()
		{
			
		}
		#endregion
	}
	
	/// <summary>
	/// Mesh class
	/// Inherits from BHElement
	/// Used for describing meshes of nodes and bars.
	/// </summary>
	public class BHMesh : BHElement
	{
		#region private members
		List<BHBar> 		m_bars = new List<BHBar>();
		ArrayList			m_nodes = new ArrayList();
		#endregion
		
		#region properties
		
		//!Returns a list of BHBars on the object.
		public List<BHBar> Bars
		{
			get
			{
				return m_bars;
			}
			set
			{
				m_bars = value;
			}
		}
		
		//!Returns a list of BHNodes on the object.
		public ArrayList Nodes
		{
			get
			{
				return m_nodes;
			}
			set
			{
				m_nodes = value;
			}
		}
		
		#endregion
		
		//!The BHMesh constructor.
		public BHMesh(int id):base(id)
		{

		}
	}

	/// <summary>
	/// WindMesh class
	/// Inherits from BHMesh
	/// Used for describing meshes of nodes and bars to be used for wind calculations.
	/// </summary>
	public class BHWindMesh : BHMesh
	{
		//create an array of indices corresponding to the location
		//in the node array of the nodes
		
		public int[] m_vertIndices;
		public float[] m_verts;
		
		/// <summary>
		/// The BHWindMesh constructor.
		/// </summary>
		/// <param name="id">unique identifier</param>
		/// <param name="bars">bars comprising the mesh</param>
		public BHWindMesh(int id, List<BHBar> bars) : base(id)
		{
			this.Bars = bars;

			List<int> vertIndexList = new List<int>();	//a list for the vert inidices
			
			foreach (BHBar b in this.Bars)				//fill up the unordered BHNode list
			{
				if (!this.Nodes.Contains(b.StartNode))
				{
					this.Nodes.Add(b.StartNode);
				}
				
				if (!this.Nodes.Contains(b.EndNode))
				{
					this.Nodes.Add(b.EndNode);
				}
			}
			
			//go through all the bars and put their nodal indices
			//into the vertex array index array
			foreach (BHBar b in this.Bars)
			{
				int nodeIndex1 = this.Nodes.IndexOf(b.StartNode);
				int nodeIndex2 = this.Nodes.IndexOf(b.EndNode);
				
				vertIndexList.Add(nodeIndex1);
				vertIndexList.Add(nodeIndex2);
			}

			m_vertIndices = vertIndexList.ToArray();
			m_verts = new float[this.Nodes.Count*3];	//initialize the array to the correct size
			//this is a straight array of x, y, and z values
			
			RebuildVertexArray();
			
		}
		
		/// <summary>
		/// Sets all nodal forces in the mesh to zero.
		/// </summary>
		public void SetAllNodalForcesToZero()
		{
			foreach (BHNode n in this.Nodes)
			{
				n.ResidualForceX = 0.0;
				n.ResidualForceY = 0.0;
				n.ResidualForceZ = 0.0;
				
				n.Stiffness = 0.0;
			}
		}
		
		public void SetAllAppliedLoadsToZero()
		{
			foreach (BHNode n in this.Nodes)
			{
				n.AppliedLoad.x = 0.0f;
				n.AppliedLoad.y = 0.0f;
				n.AppliedLoad.z = 0.0f;
			}
		}
		
		/// <summary>
		/// Rebuild the vertex array for drawing to OpenGl
		/// </summary>
		public void RebuildVertexArray()
		{
			float[] rebuiltVerts = new float[m_verts.Length];
			
			int vertCount = 0;
			
			for (int i=0; i < this.Nodes.Count; i++)
			{

				BHNode n = this.Nodes[i] as BHNode;
				rebuiltVerts[vertCount] 	= (float)n.X;
				rebuiltVerts[vertCount+1]	= (float)n.Z;	//flip the y and z for openGl
				rebuiltVerts[vertCount+2] 	= (float)n.Y;
				
				vertCount += 3;
			}
			
			m_verts = rebuiltVerts;
		}
	}
	
	/// <summary>
	/// Plate class.
	/// Inherits from BHElement.
	/// Used for describing a plate with anayltical properties
	/// </summary>
	public class BHPlate : BHElement
	{
		#region private members
		#endregion
		
		#region properties
		#endregion
		
		#region methods
		
		public BHPlate(int id) : base(id)
		{
			
		}
		
		#endregion
	}

	#region trusses
	
	public abstract class BHTruss : BHElement
	{
		protected static int m_barNum = 0;		// bar counter, used for generating id's
		
		#region BHElement members
		protected List<BHNode> 	m_topNodes 	  	= new List<BHNode>();
		protected List<BHNode>	m_bottomNodes 	= new List<BHNode>();
		protected List<BHBar> 	m_topBars 	  	= new List<BHBar>();
		protected List<BHBar> 	m_bottomBars  	= new List<BHBar>();
		protected List<BHBar>	m_laceBars 	  	= new List<BHBar>();
		#endregion
		
		#region properties
		public List<BHNode> TopNodes {
			get { return m_topNodes; }
			set { m_topNodes = value; }
		}
		public List<BHNode> BottomNodes {
			get { return m_bottomNodes; }
			set { m_bottomNodes = value; }
		}
		public List<BHBar> TopBars {
			get { return m_topBars; }
			set { m_topBars = value; }
		}
		public List<BHBar> BottomBars {
			get { return m_bottomBars; }
			set { m_bottomBars = value; }
		}
		public List<BHBar> LaceBars {
			get { return m_laceBars; }
			set { m_laceBars = value; }
		}
		#endregion
		
		#region constructors
		
		/// <summary>
		/// basic constructor
		/// </summary>
		/// <param name="id">unique identifier</param>
		/// <param name="_topNodes">ordered list of top nodes</param>
		/// <param name="_botNodes">ordered list of bottom nodes</param>
		public BHTruss(int id, List<BHNode> _topNodes, List<BHNode> _botNodes):base(id)
		{
			m_topNodes = _topNodes;
			m_bottomNodes = _botNodes;
			
			// create top bars
			for (int i=0; i<m_topNodes.Count-1; i++)
			{
				m_topBars.Add(new BHBar(m_barNum, m_topNodes[i], m_topNodes[i+1]));
				m_barNum++;
			}
			
			// create bottom bars
			for (int i=0; i<m_bottomNodes.Count-1; i++)
			{
				m_bottomBars.Add(new BHBar(m_barNum, m_bottomNodes[i], m_bottomNodes[i+1]));
				m_barNum++;
			}
		}
		
		#endregion
		
		#region unused constructors - stubbed out
		/// <summary>
		/// constructor based on a set of nodes and beam depths
		/// Not currently in use
		/// </summary>
		/// <param name="id"></param>
		/// <param name="_topNodes"></param>
		/// <param name="_startDepth"></param>
		/// <param name="_midDepth"></param>
		/// <param name="_endDepth"></param>
		public BHTruss(int id, List<BHNode> _topNodes, double _startDepth, double _midDepth, double _endDepth):base(id)
		{
			m_topNodes = _topNodes;
			
			// create top bars
			for (int i=0; i<m_topNodes.Count-1; i++)
			{
				m_topBars.Add(new BHBar(m_barNum, m_topNodes[i], m_topNodes[i+1]));
				m_barNum++;
			}
			
			// create bottom nodes
			//
			// for each topNode[i],
			// 1) find angle (alpha) between adjacent top bars
			// 2) split alpha to find down direction vector (dir)
			// 3) find depth (dBot) of corresponding bottom node by interpolating either
			//		startDepth -> midDepth or midDepth -> endDepth based on number of divisions
			// 4) bottomNode[i] = topNode[i] + (dBot * dir)
			
		}
		
		#endregion
		
		/// <summary>
		/// Function to create vertical lacing elements.
		/// Connects corresponding top and bottom nodes.
		/// </summary>
		protected void VerticalLacing()
		{
			for (int i=0; i<m_topNodes.Count; i++)
			{
				if (addPost(i))
				{
					m_laceBars.Add(new BHBar(m_barNum, m_topNodes[i], m_bottomNodes[i]));
					m_barNum++;
				}
			}
		}
		
		/// <summary>
		/// Function to create zig zag lacing elements.
		/// connects top0 -> bot1 -> top2 etc.
		/// </summary>
		protected void ZigZagLacing()
		{
			for (int i=0; i<m_topNodes.Count-1; i++)
			{
				if (addDiag(i))
				{
					if (i % 2 == 0)
						m_laceBars.Add(new BHBar(m_barNum, m_topNodes[i], m_bottomNodes[i+1]));
					else
						m_laceBars.Add(new BHBar(m_barNum, m_bottomNodes[i], m_topNodes[i+1]));
					
					m_barNum++;
				}
			}
		}
		
		/// <summary>
		/// Function to create diagonal lacing elements from top to bottom
		/// connects top0 -> bot1, top1 -> bot2, etc.
		/// </summary>
		/// <param name="start">first top node of lacing section</param>
		/// <param name="end">last top node of lacing section</param>
		protected void DiagTopToBotLacing(int first, int last)
		{
			for (int i=first; i<=last; i++)
			{
				if (addDiag(i))
				{
					m_laceBars.Add(new BHBar(m_barNum, m_topNodes[i], m_bottomNodes[i+1]));
					m_barNum++;
				}
			}
		}
		
		/// <summary>
		/// Function to create diagonal lacing elements from bottom to top
		/// connects bot0 -> top1, bot1 -> top2, etc.
		/// </summary>
		/// <param name="start">first bottom node of lacing section</param>
		/// <param name="end">last bottom node of lacing section</param>
		protected void DiagBotToTopLacing(int first, int last)
		{
			for (int i=first; i<=last; i++)
			{
				if (addDiag(i))
				{
					m_laceBars.Add(new BHBar(m_barNum, m_bottomNodes[i], m_topNodes[i+1]));
					m_barNum++;
				}
			}
		}
		
		/// <summary>
		/// tests for certain edge conditions where you would not want to add a post
		/// despite basic truss logic instructing you to do so
		/// </summary>
		/// <param name="index">index of the current node</param>
		/// <returns>true if you should add a bar, false otherwise</returns>
		private bool addPost(int index)
		{
			return !((index == 0 || index == m_topNodes.Count-1) && m_topNodes[index].AlmostEqual(m_bottomNodes[index]));
		}
		
		/// <summary>
		/// tests for certain edge conditions where you would not want to add a diagonal bar
		/// despite basic truss logic instructing you to do so
		/// </summary>
		/// <param name="index">index of the current</param>
		/// <returns>true if you should add a bar, false otherwise</returns>
		private bool addDiag(int index)
		{
			if (index == 0)	// start or end
			{
				return !m_topNodes[index].AlmostEqual(m_bottomNodes[index]);
			}
			else if (index == m_topNodes.Count-2)	// next to last (for diagonal members)
			{
				return !m_topNodes[index+1].AlmostEqual(m_bottomNodes[index+1]);
			}
			else
				return true;
		}
	}

	public class BHWarrenTruss : BHTruss
	{
		#region constructors
		public BHWarrenTruss(int id, List<BHNode> _topNodes, List<BHNode> _botNodes):base(id, _topNodes, _botNodes)
		{
			// create lacing
			VerticalLacing();
			ZigZagLacing();
		}
		#endregion
	}
	
	public class BHWarrenNoPostTruss : BHTruss
	{
		#region constructors
		public BHWarrenNoPostTruss(int id, List<BHNode> _topNodes, List<BHNode> _botNodes):base(id, _topNodes, _botNodes)
		{
			// create lacing
			ZigZagLacing();
		}
		#endregion
	}
	
	public class BHHoweTruss : BHTruss
	{
		#region constructors
		public BHHoweTruss(int id, List<BHNode> _topNodes, List<BHNode> _botNodes):base(id, _topNodes, _botNodes)
		{
			// create lacing
			VerticalLacing();
			DiagBotToTopLacing(0, m_bottomBars.Count/2 - 1);
			DiagTopToBotLacing(m_topBars.Count/2, m_topBars.Count - 1);
		}
		#endregion
		
	}
	
	public class BHPrattTruss : BHTruss
	{
		#region constructors
		public BHPrattTruss(int id, List<BHNode> _topNodes, List<BHNode> _botNodes):base(id, _topNodes, _botNodes)
		{
			// create lacing
			VerticalLacing();
			DiagTopToBotLacing(0, m_topBars.Count/2 - 1);
			DiagBotToTopLacing(m_bottomBars.Count/2, m_bottomBars.Count - 1);
		}
		#endregion
	}
	
	public class BHDiagTruss : BHTruss
	{
		#region constructors
		public BHDiagTruss(int id, List<BHNode> _topNodes, List<BHNode> _botNodes, bool topToBot):base(id, _topNodes, _botNodes)
		{
			// create lacing
			VerticalLacing();
			if (topToBot)
				DiagTopToBotLacing(0, m_bottomBars.Count - 1);
			else
				DiagBotToTopLacing(0, m_bottomBars.Count - 1);
		}
		#endregion
	}
	
	#endregion
}
