/*
 * Created by SharpDevelop.
 * User: rrosenma
 * Date: 7/17/2008
 * Time: 2:53 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;

using BuroHappold.MathUtil;
using BuroHappold.Analysis.Elements;

namespace BuroHappold.Analysis.Tensyl
{
	/// <summary>
	/// Axis of symmetry class. 
	/// Inherits from BHElement. 
	/// Used for describing an axis of symmetry for analysis.
	/// </summary>
	public class BHAxSym : BHElement
	{
		#region private members
		private Vector3 m_a;						//the first point
		private Vector3 m_b;						//the second point
		private Vector3 m_c;						//the third point
		private Vector3 m_unitNormalVector;	
		private List<BHNode> m_Nodes;
		#endregion
		
		#region properties
		
		/// <summary>
		/// Returns the unit normal vector of the axis of symmetry plane.
		/// </summary>
		public Vector3 UnitNormalVector
		{
			get
			{
				return m_unitNormalVector;
			}
			
		}
		
		/// <summary>
		/// Returns the list of nodes connected to the axis of symmetry.
		/// </summary>
		public List<BHNode> Nodes
		{
			get
			{
				return m_Nodes;
			}
		}
		
		/// <summary>
		/// Returns the first point defining the AxSym plane.
		/// </summary>
		public Vector3 p1
		{
			get
			{
				return m_a;
			}
		}
		
		/// <summary>
		/// Returns the second point defining the AxSym plane.
		/// </summary>
		public Vector3 p2
		{
			get
			{
				return m_b;
				
			}
		}
		
		/// <summary>
		/// Returns the third point defining the AxSym plane.
		/// </summary>
		public Vector3 p3
		{
			get
			{
				return m_c;
			}
		}
		
		#endregion
		
		/// <summary>
		/// Constructs a BHAxSym object from three points and a unique ID.
		/// </summary>
		/// <param name="_id">unique identifier</param>
		/// <param name="_a"></param>
		/// <param name="_b"></param>
		/// <param name="_c"></param>
		public BHAxSym(int _id, Vector3 _a, Vector3 _b, Vector3 _c) : base(_id)
		{
			m_a = _a;
			m_b = _b;
			m_c = _c;
			
			Vector3 v1 = _a.Direction(_b);
			Vector3 v2 = _a.Direction(_c);
			
			v1.Normalize();
			v2.Normalize();
	
			m_unitNormalVector = v1.CrossProduct(v1, v2);
			m_unitNormalVector.Normalize();

			m_Nodes = new List<BHNode>();	//create a new list to hold the Nodes
		}
	}

	/// <summary>
	/// Cable Property class.
	/// Inherits from BHElement
	/// Used for describing cable properties for Tensyl analysis.
	/// </summary>
	public class BHCProp : BHElement
	{
		#region private members
		private double m_ea;
		private double m_selfWeight;
		#endregion
		
		#region properties

		//!Return the stiffness value of the cable property.
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
		
		//!Return the self weight value of the cable property.
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
		
		#endregion
	
		//!Construct a BHCProp object from some analytical properties.
		public BHCProp(int _id, double _ea, double _selfWeight) : base(_id)
		{
			m_ea = _ea;
			m_selfWeight = _selfWeight;
		}
	}
}
