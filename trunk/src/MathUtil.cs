/*
 * Created by SharpDevelop.
 * User: rrosenma
 * Date: 7/14/2008
 * Time: 3:23 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Diagnostics;

namespace BuroHappold.MathUtil
{
	/// <summary>
	/// Vector Class
	/// </summary>
	public class Vector3
	{
		#region public members
		public float x;
		public float y;
		public float z;
		#endregion
		
		/// <summary>
		/// Default Vector3 constructor.
		/// </summary>
		public Vector3()
		{
			//do nothing
		}
	
		/// <summary>
		/// Construct a Vector3 object from another Vector3 object's values.
		/// </summary>
		/// <param name="a"></param>
		public Vector3(Vector3 a)
		{
			this.x = a.x;	
			this.y = a.y;	
			this.z = a.z;
		}
	
		/// <summary>
		/// Construct of Vector3 object from x,y,z points.
		/// </summary>
		/// <param name="nx">X component</param>
		/// <param name="ny">Y component</param>
		/// <param name="nz">Z component</param>
		public Vector3(float nx, float ny, float nz)
		{
			x = nx;	
			y = ny;	
			z = nz;
		}
	
		public Vector3(double nx, double ny, double nz)
		{
			x = (float)nx;
			y = (float)ny;
			z = (float)nz;
		}

		/// <summary>
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		public Vector3 Direction(Vector3 b)
		{
			// subtract the start from the end
			Vector3 directionVector = new Vector3(b.x - this.x, b.y - this.y, b.z - this.z);
			return directionVector;
		}
		
		/// <summary>
		/// Set the Vector3's components to that of another Vector3 object.
		/// </summary>
		/// <param name="a">Vector3 which is the source</param>
		public void SetEqualTo(Vector3 a) 
		{
			x = a.x; 
			y = a.y; 
			z = a.z;

		}
	
		/// <summary>
		/// Returns whether this Vector3's components are equal to those of another.
		/// </summary>
		/// <param name="a">The vector being compared against</param>
		/// <returns>true if equal, false otherwise</returns>
		public bool IsEqualTo(Vector3 a)	// changed from operator == for c# compliance
		{
			return x == a.x && y == a.y && z == a.z;
		}
	
		/// <summary>
		/// Returns whether this Vector3's components are not equal to those of another.
		/// </summary>
		/// <param name="a">The vector being compared against</param>
		/// <returns>true if not equal, false otherwise</returns>
		public bool IsNotEqualTo(Vector3 a) 	//changed from operator != for c# compliance
		{
			return x != a.x || y != a.y || z != a.z;
		}
	
		#region vector operations
	
		//!Sets the vector to zero.
		public void Zero() 
		{ 
			x = y = z = 0.0f; 
		}
	
		//!Unary minus returns the negative of the vector
		public Vector3 Negate()	//changed from "operator -" to avoid confusion with minus operator
		{ 
			return new Vector3(-x,-y,-z); 
		}
	
		//!Add one vector to another.
//		public static Vector3 operator +(Vector3 a, Vector3 b)
		public Vector3 Add(Vector3 a)
		{
			return new Vector3(x + a.x, y + a.y, z + a.z);
		}
	
		//!Subtract one vector from another.
//		public static Vector3 operator -(Vector3 a, Vector3 b)
		public Vector3 Subtract(Vector3 a)
		{
			return new Vector3(x - a.x, y - a.y, z - a.z);
		}
	
		//!Multiply the vector by a scalar.
		public Vector3 Multiply(float a)
//		public static Vector3 operator *(Vector3 a, float b)
		{
			return new Vector3(x*a, y*a, z*a);
		}
	
		//*Divide the vector by a scalar.
		public Vector3 Divide(float a)
//		public static Vector3 operator/(Vector3 a, float b)
		{
			float	oneOverA = 1.0f / a; // NOTE: no check for divide by zero here
			return new Vector3(x*oneOverA, y*oneOverA, z*oneOverA);
		}
	
		#region dead operator overrides
		
		// Combined assignment operators to conform to
		// C notation convention
	
//		public static Vector3 operator +=(Vector3 a) 
//		{
//			x += a.x; y += a.y; z += a.z;
//			return this;
//		}
//	
//		public static Vector3 operator -=(Vector3 a) 
//		{
//			x -= a.x; y -= a.y; z -= a.z;
//			return this;
//		}
//	
//		public static Vector3 operator *=(float a) 
//		{
//			x *= a; y *= a; z *= a;
//			return *this;
//		}
	
//		public static Vector3 operator /=(float a) 
//		{
//			float	oneOverA = 1.0f / a;
//			x *= oneOverA; 
//			y *= oneOverA; 
//			z *= oneOverA;
//			return this;
//		}
	
		#endregion
		
		//!Normalize the vector.
		public void Normalize() 
		{
			float magSq = x*x + y*y + z*z;
			if (magSq > 0.0f) 
			{ // check for divide-by-zero
				float oneOverMag = (1.0f / (float)System.Math.Sqrt(magSq));
				x *= oneOverMag;
				y *= oneOverMag;
				z *= oneOverMag;
			}
		}
	
		//!*Vector dot product.  We overload the standard
		// multiplication symbol to do this.*!
		public float DotProduct(Vector3 a)
		{
			return x*a.x + y*a.y + z*a.z;
		}
		
		//!Compute the magnitude of a vector.
		public float VectorMag(Vector3 a) 
		{
			return (float) System.Math.Sqrt(a.x*a.x + a.y*a.y + a.z*a.z);
		}
		
		//!Compute the cross product of two vectors.
		public Vector3 CrossProduct(Vector3 a, Vector3 b) 
		{
			return new Vector3(
				a.y*b.z - a.z*b.y,
				a.z*b.x - a.x*b.z,
				a.x*b.y - a.y*b.x
				);
		}
		
		//!Compute the distance between two points.
		public float Distance(Vector3 a, Vector3 b) 
		{
			float dx = a.x - b.x;
			float dy = a.y - b.y;
			float dz = a.z - b.z;
			return (float)System.Math.Sqrt(dx*dx + dy*dy + dz*dz);
		
		
		}
		
		//!Convert the vector to an object array.
		public object[] ToArray(Vector3 v)
		{
			object[] outArr = new object[3]{v.x, v.y, v.z};
			return outArr;
		}
		#endregion
		
	}
	
	public class Vector2
	{
		public float x;
		public float y;
		
		/// <summary>
		/// Construct a Vector3 object from another Vector3 object's values.
		/// </summary>
		/// <param name="a"></param>
		public Vector2(Vector2 a)
		{
			this.x = a.x;	
			this.y = a.y;
		}
	
		/// <summary>
		/// Construct of Vector3 object from x,y,z points.
		/// </summary>
		/// <param name="nx">X component</param>
		/// <param name="ny">Y component</param>
		public Vector2(float nx, float ny)
		{
			x = nx;	
			y = ny;
		}
	
		public Vector2(double nx, double ny)
		{
			x = (float)nx;
			y = (float)ny;
		}
		
		/// <summary>
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		public Vector2 Direction(Vector2 b)
		{
			// subtract the start from the end
			return new Vector2(b.x - this.x, b.y - this.y);
		}
		
		/// <summary>
		/// Set the Vector2's components to that of another Vector2 object.
		/// </summary>
		/// <param name="a">Vector2 which is the source</param>
		public void SetEqualTo(Vector2 a) 
		{
			x = a.x; 
			y = a.y;
		}
	
		/// <summary>
		/// Returns whether this Vector2's components are equal to those of another.
		/// </summary>
		/// <param name="a">The vector being compared against</param>
		/// <returns>true if equal, false otherwise</returns>
		public bool IsEqualTo(Vector2 a)	// changed from operator == for c# compliance
		{
			return x == a.x && y == a.y;
		}
	
		/// <summary>
		/// Returns whether this Vector2's components are not equal to those of another.
		/// </summary>
		/// <param name="a">The vector being compared against</param>
		/// <returns>true if not equal, false otherwise</returns>
		public bool IsNotEqualTo(Vector2 a) 	//changed from operator != for c# compliance
		{
			return x != a.x || y != a.y;
		}
	
		//!Sets the vector to zero.
		public void Zero() 
		{ 
			x = y = 0.0f; 
		}
	
		//!Unary minus returns the negative of the vector
		public Vector2 Negate()	//changed from "operator -" to avoid confusion with minus operator
		{ 
			return new Vector2(-x,-y); 
		}
	
		//!Add one vector to another.
//		public static Vector2 operator +(Vector2 a, Vector2 b)
		public Vector2 Add(Vector2 a)
		{
			return new Vector2(x + a.x, y + a.y);
		}
	
		//!Subtract one vector from another.
//		public static Vector2 operator -(Vector2 a, Vector2 b)
		public Vector2 Subtract(Vector2 a)
		{
			return new Vector2(x - a.x, y - a.y);
		}
	
		//!Multiply the vector by a scalar.
		public Vector2 Multiply(float a)
		{
			return new Vector2(x*a, y*a);
		}
	
		//*Divide the vector by a scalar.
		public Vector2 Divide(float a)
//		public static Vector2 operator/(Vector2 a, float b)
		{
			float	oneOverA = 1.0f / a; // NOTE: no check for divide by zero here
			return new Vector2(x*oneOverA, y*oneOverA);
		}
		
		//!Normalize the vector.
		public void Normalize() 
		{
			float magSq = x*x + y*y;
			if (magSq > 0.0f) 
			{ // check for divide-by-zero
				float oneOverMag = (1.0f / (float)System.Math.Sqrt(magSq));
				x *= oneOverMag;
				y *= oneOverMag;
			}
		}
	
		//!*Vector dot product.  We overload the standard
		// multiplication symbol to do this.*!
		public float DotProduct(Vector2 a)
		{
			return x*a.x + y*a.y;
		}
		
		//!Compute the magnitude of a vector.
		public float Magnitude() 
		{
			return (float) System.Math.Sqrt(this.x*this.x + this.y*this.y);
		}
		
		//!Compute the magnitude of a vector.
		public static float Magnitude(Vector2 a) 
		{
			return (float) System.Math.Sqrt(a.x*a.x + a.y*a.y);
		}
		
		//!Compute the distance between two points.
		public static float Distance(Vector2 a, Vector2 b) 
		{
			float dx = a.x - b.x;
			float dy = a.y - b.y;
			return (float) System.Math.Sqrt(dx*dx + dy*dy);
		
		
		}
		
		
//		public static double DotProduct(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
//		{
//			double BAx, BAy, BCx, BCy;
//			
//			BAx = Ax - Bx;
//    		BAy = Ay - By;
//    		BCx = Cx - Bx;
//    		BCy = Cy - By;
//    		
//    		// calculate dot product
//    		return BAx * BCx + BAy * BCy;
//		}
//		
//		public static double Magnitude(double Ax, double Ay, double Bx, double By)
//		{
//			double BAx, BAy;
//			
//			BAx = Ax - Bx;
//    		BAy = Ay - By;
//    		
//    		return Math.Sqrt(BAx*BAx + BAy*BAy);
//		}
	}
	
	/// <summary>
	/// 4x3 Matrix class. 
	/// Used for calculating affine transformations.
	/// </summary>
	public class Matrix4x3
	{
		/////////////////////////////////////////////////////////////////////////////
		//
		// 3D Math Primer for Games and Graphics Development
		//
		// Matrix4x3.cpp - Implementation of class Matrix4x3
		//
		// Visit gamemath.com for the latest version of this file.
		//
		// For more details see section 11.5.
		//
		/////////////////////////////////////////////////////////////////////////////

		/////////////////////////////////////////////////////////////////////////////
		//
		// Notes:
		//
		// See Chapter 11 for more information on class design decisions.
		//
		//---------------------------------------------------------------------------
		//
		// MATRIX ORGANIZATION
		//
		// The purpose of this class is so that a user might perform transformations
		// without fiddling with plus or minus signs or transposing the matrix
		// until the output "looks right."  But of course, the specifics of the
		// internal representation is important.  Not only for the implementation
		// in this file to be correct, but occasionally direct access to the
		// matrix variables is necessary, or beneficial for optimization.  Thus,
		// we document our matrix conventions here.
		//
		// We use row vectors, so multiplying by our matrix looks like this:
		//
		//               | m11 m12 m13 |
		//     [ x y z ] | m21 m22 m23 | = [ x' y' z' ]
		//               | m31 m32 m33 |
		//               | tx  ty  tz  |
		//
		// Strict adherance to linear algebra rules dictates that this
		// multiplication is actually undefined.  To circumvent this, we can
		// consider the input and output vectors as having an assumed fourth
		// coordinate of 1.  Also, since we cannot technically invert a 4x3 matrix
		// according to linear algebra rules, we will also assume a rightmost
		// column of [ 0 0 0 1 ].  This is shown below:
		//
		//                 | m11 m12 m13 0 |
		//     [ x y z 1 ] | m21 m22 m23 0 | = [ x' y' z' 1 ]
		//                 | m31 m32 m33 0 |
		//                 | tx  ty  tz  1 |
		//
		// In case you have forgotten your linear algebra rules for multiplying
		// matrices (which are described in section 7.1.6 and 7.1.7), see the
		// definition of operator* for the expanded computations.
		//
		/////////////////////////////////////////////////////////////////////////////
		
		#region public members
		
		public float	m11, m12, m13;
		public float	m21, m22, m23;
		public float	m31, m32, m33;
		public float	tx,  ty,  tz;
		
		#endregion
		
		#region private members
		private MathUtil mathUtil = new MathUtil();
		private EulerAngles eulerAngles = new EulerAngles();
		private Quaternion quaternion = new Quaternion();
		private RotationMatrix rotationMatrix = new RotationMatrix();
		#endregion
		
		#region methods
		
		/// <summary>
		/// Set the matrix to identity
		/// </summary>
		public void Identity() 
		{
			m11 = 1.0f; m12 = 0.0f; m13 = 0.0f;
			m21 = 0.0f; m22 = 1.0f; m23 = 0.0f;
			m31 = 0.0f; m32 = 0.0f; m33 = 1.0f;
			tx  = 0.0f; ty  = 0.0f; tz  = 1.0f;
		}
		
		/// <summary>
		/// Zero the 4th row of the matrix, which contains the translation portion.
		/// </summary>
		public void ZeroTranslation() 
		{
			tx = ty = tz = 0.0f;
		}
		
		/// <summary>
		/// Sets the translation portion of the matrix in vector form
		/// </summary>
		/// <param name="d">the translation vector</param>
		public void SetTranslation(Vector3 d) 
		{
			tx = d.x; 
			ty = d.y; 
			tz = d.z;
		}
		
		/// <summary>
		/// Sets the translation portion of the matrix in vector form
		/// </summary>
		/// <param name="d"></param>
		public void SetupTranslation(Vector3 d) 
		{
			// Set the linear transformation portion to identity
			m11 = 1.0f; m12 = 0.0f; m13 = 0.0f;
			m21 = 0.0f; m22 = 1.0f; m23 = 0.0f;
			m31 = 0.0f; m32 = 0.0f; m33 = 1.0f;
		
			// Set the translation portion
			tx = d.x; ty = d.y; tz = d.z;
		}
		
		/// <summary>
		/// Setup the matrix to perform a local -> parent transformation, given
		/// the position and orientation of the local reference frame within the
		/// parent reference frame.
		///
		/// A very common use of this will be to construct a object -> world matrix.
		/// As an example, the transformation in this case is straightforward.  We
		/// first rotate from object space into inertial space, then we translate
		/// into world space.
		///
		/// We allow the orientation to be specified using either euler angles,
		/// or a RotationMatrix
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="orient"></param>
		public void SetupLocalToParent(Vector3 pos, EulerAngles orient) 
		{
			// Create a rotation matrix.
			RotationMatrix orientMatrix = new RotationMatrix();
			orientMatrix.Setup(orient);
		
			// Setup the 4x3 matrix.  Note: if we were really concerned with
			// speed, we could create the matrix directly into these variables,
			// without using the temporary RotationMatrix object.  This would
			// save us a function call and a few copy operations.
			
			SetupLocalToParent(pos, orientMatrix);
		}
		
		public void SetupLocalToParent(Vector3 pos, RotationMatrix orient) 
		{
			// Copy the rotation portion of the matrix.  According to
			// the comments in RotationMatrix.cpp, the rotation matrix
			// is "normally" an inertial->object matrix, which is
			// parent->local.  We want a local->parent rotation, so we
			// must transpose while copying
		
			m11 = orient.m11; m12 = orient.m21; m13 = orient.m31;
			m21 = orient.m12; m22 = orient.m22; m23 = orient.m32;
			m31 = orient.m13; m32 = orient.m23; m33 = orient.m33;
		
			// Now set the translation portion.  Translation happens "after"
			// the 3x3 portion, so we can simply copy the position
			// field directly
		
			tx = pos.x; ty = pos.y; tz = pos.z;
		}
		
		/// <summary>
		/// Setup the matrix to perform a parent -> local transformation, given
		/// the position and orientation of the local reference frame within the
		/// parent reference frame.
		///
		/// A very common use of this will be to construct a world -> object matrix.
		/// To perform this transformation, we would normally FIRST transform
		/// from world to inertial space, and then rotate from inertial space into
		/// object space.  However, out 4x3 matrix always translates last.  So
		/// we think about creating two matrices T and R, and then concatonating
		/// M = TR.
		///
		/// We allow the orientation to be specified using either euler angles,
		/// or a RotationMatrix
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="orient"></param>
		public void SetupParentToLocal(Vector3 pos, EulerAngles orient) 
		{
			// Create a rotation matrix.
			RotationMatrix orientMatrix = new RotationMatrix();
			orientMatrix.Setup(orient);
		
			// Setup the 4x3 matrix.
		
			SetupParentToLocal(pos, orientMatrix);
		}
		
		public void SetupParentToLocal(Vector3 pos, RotationMatrix orient) 
		{
		
			// Copy the rotation portion of the matrix.  We can copy the
			// elements directly (without transposing) according
			// to the layout as commented in RotationMatrix.cpp
		
			m11 = orient.m11; m12 = orient.m12; m13 = orient.m13;
			m21 = orient.m21; m22 = orient.m22; m23 = orient.m23;
			m31 = orient.m31; m32 = orient.m32; m33 = orient.m33;
		
			// Now set the translation portion.  Normally, we would
			// translate by the negative of the position to translate
			// from world to inertial space.  However, we must correct
			// for the fact that the rotation occurs "first."  So we
			// must rotate the translation portion.  This is the same
			// as create a translation matrix T to translate by -pos,
			// and a rotation matrix R, and then creating the matrix
			// as the concatenation of TR
		
			tx = -(pos.x*m11 + pos.y*m21 + pos.z*m31);
			ty = -(pos.x*m12 + pos.y*m22 + pos.z*m32);
			tz = -(pos.x*m13 + pos.y*m23 + pos.z*m33);
		}
		
		/// <summary>
		/// Setup the matrix to perform a rotation about a cardinal axis
		///
		/// The axis of rotation is specified using a 1-based index:
		///
		///	1 => rotate about the x-axis
		///	2 => rotate about the y-axis
		///	3 => rotate about the z-axis
		///
		/// theta is the amount of rotation, in radians.  The left-hand rule is
		/// used to define "positive" rotation.
		///
		/// The translation portion is reset.
		/// 
		/// See 8.2.2 for more info.
		/// </summary>
		/// <param name="axis"></param>
		/// <param name="theta"></param>
		public void SetupRotate(int axis, float theta) 
		{
			// Get sin and cosine of rotation angle
			float s = 0;
			float c = 0;
			mathUtil.SinCos(ref s, ref c, theta);
		
			// Check which axis they are rotating about
		
			switch (axis) {
		
				case 1: // Rotate about the x-axis
		
					m11 = 1.0f; m12 = 0.0f; m13 = 0.0f;
					m21 = 0.0f; m22 = c;    m23 = s;
					m31 = 0.0f; m32 = -s;   m33 = c;
					break;
		
				case 2: // Rotate about the y-axis
		
					m11 = c;    m12 = 0.0f; m13 = -s;
					m21 = 0.0f; m22 = 1.0f; m23 = 0.0f;
					m31 = s;    m32 = 0.0f; m33 = c;
					break;
		
				case 3: // Rotate about the z-axis
		
					m11 = c;    m12 = s;    m13 = 0.0f;
					m21 = -s;   m22 = c;    m23 = 0.0f;
					m31 = 0.0f; m32 = 0.0f; m33 = 1.0f;
					break;
		
				default:

					Debug.Assert(false);
					break;

			}
		
			// Reset the translation portion
		
			tx = ty = tz = 0.0f;
		}
		
		/// <summary>
		/// Setup the matrix to perform a rotation about an arbitrary axis.
		/// The axis of rotation must pass through the origin.
		///
		/// axis defines the axis of rotation, and must be a unit vector.
		///
		/// theta is the amount of rotation, in radians.  The left-hand rule is
		/// used to define "positive" rotation.
		///
		/// The translation portion is reset.
		/// 
		/// See 8.2.3 for more info.
		/// </summary>
		/// <param name="axis"></param>
		/// <param name="theta"></param>
		public void SetupRotate(Vector3 axis, float theta) 
		{
			// Quick sanity check to make sure they passed in a unit vector
			// to specify the axis
		
			Debug.Assert(System.Math.Abs(axis.DotProduct(axis) - 1.0f) < .01f);
		
			// Get sin and cosine of rotation angle
		
			float s = 0;
			float c = 0;
			mathUtil.SinCos(ref s, ref c, theta);
		
			// Compute 1 - cos(theta) and some common subexpressions
		
			float	a = 1.0f - c;
			float	ax = a * axis.x;
			float	ay = a * axis.y;
			float	az = a * axis.z;
		
			// Set the matrix elements.  There is still a little more
			// opportunity for optimization due to the many common
			// subexpressions.  We'll let the compiler handle that...
		
			m11 = ax*axis.x + c;
			m12 = ax*axis.y + axis.z*s;
			m13 = ax*axis.z - axis.y*s;
		
			m21 = ay*axis.x - axis.z*s;
			m22 = ay*axis.y + c;
			m23 = ay*axis.z + axis.x*s;
		
			m31 = az*axis.x + axis.y*s;
			m32 = az*axis.y - axis.x*s;
			m33 = az*axis.z + c;
		
			// Reset the translation portion
		
			tx = ty = tz = 0.0f;
		}
		
		/// <summary>
		/// Matrix4x3::fromQuaternion
		///
		/// Setup the matrix to perform a rotation, given the angular displacement
		/// in quaternion form.
		///
		/// The translation portion is reset.
		///
		/// See 10.6.3 for more info.
		/// </summary>
		/// <param name="q">Quaternion</param>
		public void FromQuaternion(Quaternion q) 
		{
			// Compute a few values to optimize common subexpressions
		
			float	ww = 2.0f * q.w;
			float	xx = 2.0f * q.x;
			float	yy = 2.0f * q.y;
			float	zz = 2.0f * q.z;
		
			// Set the matrix elements.  There is still a little more
			// opportunity for optimization due to the many common
			// subexpressions.  We'll let the compiler handle that...
		
			m11 = 1.0f - yy*q.y - zz*q.z;
			m12 = xx*q.y + ww*q.z;
			m13 = xx*q.z - ww*q.x;
		
			m21 = xx*q.y - ww*q.z;
			m22 = 1.0f - xx*q.x - zz*q.z;
			m23 = yy*q.z + ww*q.x;
		
			m31 = xx*q.z + ww*q.y;
			m32 = yy*q.z - ww*q.x;
			m33 = 1.0f - xx*q.x - yy*q.y;
		
			// Reset the translation portion
		
			tx = ty = tz = 0.0f;
		}
		
		/// <summary>
		/// Setup the matrix to perform scale on each axis.  For uniform scale by k,
		/// use a vector of the form Vector3(k,k,k)
		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>/
		/// The translation portion is reset.
		///
		/// See 8.3.1 for more info. *!
		/// </summary>
		/// <param name="s"></param>
		public void SetupScale(Vector3 s) 
		{
			// Set the matrix elements.  Pretty straightforward
			m11 = s.x;  m12 = 0.0f; m13 = 0.0f;
			m21 = 0.0f; m22 = s.y;  m23 = 0.0f;
			m31 = 0.0f; m32 = 0.0f; m33 = s.z;
		
			// Reset the translation portion
			tx = ty = tz = 0.0f;
		}
		
		/// <summary>
		/// Setup the matrix to perform a scale along an arbitrary axis.
		///  - The axis is specified using a unit vector.
		///  - The translation portion is reset.
		///
		/// See 8.3.2 for more info.
		/// </summary>
		/// <param name="axis">the vector representing the axis </param>
		/// <param name="k"></param>
		public void SetupScaleAlongAxis(Vector3 axis, float k) 
		{
			// Quick sanity check to make sure they passed in a unit vector
			// to specify the axis
		
			Debug.Assert(System.Math.Abs(axis.DotProduct(axis) - 1.0f) < .01f);
		
			// Compute k-1 and some common subexpressions
		
			float	a = k - 1.0f;
			float	ax = a * axis.x;
			float	ay = a * axis.y;
			float	az = a * axis.z;
		
			// Fill in the matrix elements.  We'll do the common
			// subexpression optimization ourselves here, since diagonally
			// opposite matrix elements are equal
		
			m11 = ax*axis.x + 1.0f;
			m22 = ay*axis.y + 1.0f;
			m32 = az*axis.z + 1.0f;
		
			m12 = m21 = ax*axis.y;
			m13 = m31 = ax*axis.z;
			m23 = m32 = ay*axis.z;
		
			// Reset the translation portion
		
			tx = ty = tz = 0.0f;
		}
		
		/// <summary>
		/// Setup the matrix to perform a shear
		//
		/// The type of shear is specified by the 1-based "axis" index.  The effect
		/// of transforming a point by the matrix is described by the pseudocode
		/// below:
		///
		///	axis == 1  =>  y += s*x, z += t*x
		///	axis == 2  =>  x += s*y, z += t*y
		///	axis == 3  =>  x += s*z, y += t*z
		///
		/// The translation portion is reset.
		///
		/// See 8.6 for more info.
		/// </summary>
		/// <param name="axis"></param>
		/// <param name="s"></param>
		/// <param name="t"></param>
		public void SetupShear(int axis, float s, float t) 
		{
			// Check which type of shear they want
		
			switch (axis) {
		
				case 1: // Shear y and z using x
		
					m11 = 1.0f; m12 = s;    m13 = t;
					m21 = 0.0f; m22 = 1.0f; m23 = 0.0f;
					m31 = 0.0f; m32 = 0.0f; m33 = 1.0f;
					break;
		
				case 2: // Shear x and z using y
		
					m11 = 1.0f; m12 = 0.0f; m13 = 0.0f;
					m21 = s;    m22 = 1.0f; m23 = t;
					m31 = 0.0f; m32 = 0.0f; m33 = 1.0f;
					break;
		
				case 3: // Shear x and y using z
		
					m11 = 1.0f; m12 = 0.0f; m13 = 0.0f;
					m21 = 0.0f; m22 = 1.0f; m23 = 0.0f;
					m31 = s;    m32 = t;    m33 = 1.0f;
					break;
		
				default:
		
					// bogus axis index
					Debug.Assert(false);
					break;
			}
		
			// Reset the translation portion
			tx = ty = tz = 0.0f;
		}
		
		/// <summary>
		/// Setup the matrix to perform a projection onto a plane passing
		/// through the origin.  The plane is perpendicular to the
		/// unit vector n.
		///
		/// See 8.4.2 for more info.
		/// </summary>
		/// <param name="n"></param>
		public void SetupProject(Vector3 n) 
		{
			// Quick sanity check to make sure they passed in a unit vector
			// to specify the axis
		
			float debugDotProduct = n.DotProduct(n);
			Debug.Assert(System.Math.Abs(n.DotProduct(n) - 1.0f) < .01f);
		
			// Fill in the matrix elements.  We'll do the common
			// subexpression optimization ourselves here, since diagonally
			// opposite matrix elements are equal
		
			m11 = 1.0f - n.x*n.x;
			m22 = 1.0f - n.y*n.y;
			m33 = 1.0f - n.z*n.z;
		
			m12 = m21 = -n.x*n.y;
			m13 = m31 = -n.x*n.z;
			m23 = m32 = -n.y*n.z;
		
			// Reset the translation portion
		
			tx = ty = tz = 0.0f;
		}
		
		/// <summary>
		/// Setup the matrix to perform a reflection about a plane parallel
		/// to a cardinal plane.
		///
		/// axis is a 1-based index which specifies the plane to project about:
		///
		///	1 => reflect about the plane x=k
		///	2 => reflect about the plane y=k
		///	3 => reflect about the plane z=k
		///
		/// The translation is set appropriately, since translation must occur if
		/// k != 0
		///
		/// See 8.5 for more info.
		/// </summary>
		/// <param name="axis"></param>
		/// <param name="k"></param>
		public void SetupReflectParallelToCardinal(int axis, float k) 
		{
			// Check which plane they want to reflect about
		
			switch (axis) {
		
				case 1: // Reflect about the plane x=k
		
					m11 = -1.0f; m12 =  0.0f; m13 =  0.0f;
					m21 =  0.0f; m22 =  1.0f; m23 =  0.0f;
					m31 =  0.0f; m32 =  0.0f; m33 =  1.0f;
		
					tx = 2.0f * k;
					ty = 0.0f;
					tz = 0.0f;
		
					break;
		
				case 2: // Reflect about the plane y=k
		
					m11 =  1.0f; m12 =  0.0f; m13 =  0.0f;
					m21 =  0.0f; m22 = -1.0f; m23 =  0.0f;
					m31 =  0.0f; m32 =  0.0f; m33 =  1.0f;
		
					tx = 0.0f;
					ty = 2.0f * k;
					tz = 0.0f;
		
					break;
		
				case 3: // Reflect about the plane z=k
		
					m11 =  1.0f; m12 =  0.0f; m13 =  0.0f;
					m21 =  0.0f; m22 =  1.0f; m23 =  0.0f;
					m31 =  0.0f; m32 =  0.0f; m33 = -1.0f;
		
					tx = 0.0f;
					ty = 0.0f;
					tz = 2.0f * k;
		
					break;
		
				default:
		
					// bogus axis index
					Debug.Assert(false);
					break;
			}
		}
		
		/// <summary>
		/// Setup the matrix to perform a reflection about an arbitrary plane
		/// through the origin.  The unit vector n is perpendicular to the plane.
		///
		/// The translation portion is reset.
		///
		/// See 8.5 for more info.
		/// </summary>
		/// <param name="n">unit vector perpendicular to the plane</param>
		public void SetupReflectArbitrary(Vector3 n) 
		{
			// Quick sanity check to make sure they passed in a unit vector
			// to specify the axis
	
			float debugDotProduct = n.DotProduct(n);
			Debug.Assert(System.Math.Abs(n.DotProduct(n) - 1.0f) < .01f);
		
			// Compute common subexpressions
		
			float	ax = -2.0f * n.x;
			float	ay = -2.0f * n.y;
			float	az = -2.0f * n.z;
		
			// Fill in the matrix elements.  We'll do the common
			// subexpression optimization ourselves here, since diagonally
			// opposite matrix elements are equal
		
			m11 = 1.0f + ax*n.x;
			m22 = 1.0f + ay*n.y;
			m32 = 1.0f + az*n.z;
		
			m12 = m21 = ax*n.y;
			m13 = m31 = ax*n.z;
			m23 = m32 = ay*n.z;
		
			// Reset the translation portion
		
			tx = ty = tz = 0.0f;
		}
		
		/// <summary>
		/// Vector * Matrix4x3
		/// 
		/// Transforms a point.  
		/// This makes using the vector class look like it does with linear algebra notation on paper.
		/// </summary>
		/// <param name="p">the point</param>
		/// <param name="m">the transformation matrix</param>
		/// <returns></returns>
		public static Vector3 operator *(Vector3 p, Matrix4x3 m)
		{
			// Grind through the linear algebra.
			return new Vector3(
				p.x*m.m11 + p.y*m.m21 + p.z*m.m31 + m.tx,
				p.x*m.m12 + p.y*m.m22 + p.z*m.m32 + m.ty,
				p.x*m.m13 + p.y*m.m23 + p.z*m.m33 + m.tz
			);
		}
		
		/// <summary>
		/// Multiplies two matrices together
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Matrix4x3 operator *(Matrix4x3 a, Matrix4x3 b) 
		{
		
			Matrix4x3 r = new Matrix4x3();
		
			// Compute the upper 3x3 (linear transformation) portion
		
			r.m11 = a.m11*b.m11 + a.m12*b.m21 + a.m13*b.m31;
			r.m12 = a.m11*b.m12 + a.m12*b.m22 + a.m13*b.m32;
			r.m13 = a.m11*b.m13 + a.m12*b.m23 + a.m13*b.m33;
		
			r.m21 = a.m21*b.m11 + a.m22*b.m21 + a.m23*b.m31;
			r.m22 = a.m21*b.m12 + a.m22*b.m22 + a.m23*b.m32;
			r.m23 = a.m21*b.m13 + a.m22*b.m23 + a.m23*b.m33;
		
			r.m31 = a.m31*b.m11 + a.m32*b.m21 + a.m33*b.m31;
			r.m32 = a.m31*b.m12 + a.m32*b.m22 + a.m33*b.m32;
			r.m33 = a.m31*b.m13 + a.m32*b.m23 + a.m33*b.m33;
		
			// Compute the translation portion
		
			r.tx = a.tx*b.m11 + a.ty*b.m21 + a.tz*b.m31 + b.tx;
			r.ty = a.tx*b.m12 + a.ty*b.m22 + a.tz*b.m32 + b.ty;
			r.tz = a.tx*b.m13 + a.ty*b.m23 + a.tz*b.m33 + b.tz;
		
			// Return it.  Ouch - involves a copy constructor call.  If speed
			// is critical, we may need a seperate function which places the
			// result where we want it...
		
			return r;
		}
		
		/// <summary>
		/// Compute the determinant of the 3x3 portion of the matrix.
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public float Determinant(Matrix4x3 m) 
		{
			return
				  m.m11 * (m.m22*m.m33 - m.m23*m.m32)
				+ m.m12 * (m.m23*m.m31 - m.m21*m.m33)
				+ m.m13 * (m.m21*m.m32 - m.m22*m.m31);
		}
		
		/// <summary>
		/// Computes the inverse of a matrix.  
		/// We use the classical adjoint divided by the determinant method.
		/// </summary>
		/// <param name="m">matrix</param>
		/// <returns></returns>
		public Matrix4x3 Inverse(Matrix4x3 m) 
		{
			// Compute the determinant
		
			float	det = Determinant(m);
		
			// If we're singular, then the determinant is zero and there's
			// no inverse
		
			Debug.Assert(System.Math.Abs(det) > 0.000001f);
		
			// Compute one over the determinant, so we divide once and
			// can *multiply* per element
		
			float	oneOverDet = 1.0f / det;
		
			// Compute the 3x3 portion of the inverse, by
			// dividing the adjoint by the determinant
		
			Matrix4x3	r = new Matrix4x3();
		
			r.m11 = (m.m22*m.m33 - m.m23*m.m32) * oneOverDet;
			r.m12 = (m.m13*m.m32 - m.m12*m.m33) * oneOverDet;
			r.m13 = (m.m12*m.m23 - m.m13*m.m22) * oneOverDet;
		
			r.m21 = (m.m23*m.m31 - m.m21*m.m33) * oneOverDet;
			r.m22 = (m.m11*m.m33 - m.m13*m.m31) * oneOverDet;
			r.m23 = (m.m13*m.m21 - m.m11*m.m23) * oneOverDet;
		
			r.m31 = (m.m21*m.m32 - m.m22*m.m31) * oneOverDet;
			r.m32 = (m.m12*m.m31 - m.m11*m.m32) * oneOverDet;
			r.m33 = (m.m11*m.m22 - m.m12*m.m21) * oneOverDet;
		
			// Compute the translation portion of the inverse
		
			r.tx = -(m.tx*r.m11 + m.ty*r.m21 + m.tz*r.m31);
			r.ty = -(m.tx*r.m12 + m.ty*r.m22 + m.tz*r.m32);
			r.tz = -(m.tx*r.m13 + m.ty*r.m23 + m.tz*r.m33);
		
			// Return it.  Ouch - involves a copy constructor call.  If speed
			// is critical, we may need a seperate function which places the
			// result where we want it...
		
			return r;
		}
		
		/// <summary>
		/// Return the translation row of the matrix in vector form
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public Vector3 GetTranslation(Matrix4x3 m) 
		{
			return new Vector3(m.tx, m.ty, m.tz);
		}
		
		/// <summary>
		/// Extract the position of an object given a parent -> local transformation matrix 
		/// (i.e. a world -> object matrix)
		///
		/// We assume that the matrix represents a rigid transformation.  
		/// (i.e. no scale, skew, or mirroring)
		/// </summary>
		/// <param name="m">a parent -> local transformation matrix</param>
		/// <returns></returns>
		public Vector3 GetPositionFromParentToLocalMatrix(Matrix4x3 m) 
		{
		
			// Multiply negative translation value by the
			// transpose of the 3x3 portion.  By using the transpose,
			// we assume that the matrix is orthogonal.  (This function
			// doesn't really make sense for non-rigid transformations...)
		
			return new Vector3(
				-(m.tx*m.m11 + m.ty*m.m12 + m.tz*m.m13),
				-(m.tx*m.m21 + m.ty*m.m22 + m.tz*m.m23),
				-(m.tx*m.m31 + m.ty*m.m32 + m.tz*m.m33)
			);
		}
		
		/// <summary>
		/// Extract the position of an object given a local -> parent transformation matrix
		/// (i.e. an object -> world matrix)
		/// </summary>
		/// <param name="m">a local -> parent transformation matrix</param>
		/// <returns></returns>
		public Vector3 GetPositionFromLocalToParentMatrix(Matrix4x3 m) 
		{
			// Position is simply the translation portion
			return new Vector3(m.tx, m.ty, m.tz);
		}
		
		#endregion

	}

	/// <summary>
	/// Rotation matric class
	/// </summary>
	public class RotationMatrix
	{
		Vector3 vec3 = new Vector3();
		EulerAngles eulAng = new EulerAngles();
		Quaternion quat = new Quaternion();
		MathUtil mathUtil = new MathUtil();
		
		//! MATRIX ORGANIZATION
		//
		// A user of this class should rarely care how the matrix is organized.
		// However, it is of course important that internally we keep everything
		// straight.
		//
		// The matrix is assumed to be a rotation matrix only, and therefore
		// orthoganal.  The "forward" direction of transformation (if that really
		// even applies in this case) will be from inertial to object space.
		// To perform an object->inertial rotation, we will multiply by the
		// transpose.
		//
		// In other words:
		//
		// Inertial to object:
		//
		//                  | m11 m12 m13 |
		//     [ ix iy iz ] | m21 m22 m23 | = [ ox oy oz ]
		//                  | m31 m32 m33 |
		//
		// Object to inertial:
		//
		//                  | m11 m21 m31 |
		//     [ ox oy oz ] | m12 m22 m32 | = [ ix iy iz ]
		//                  | m13 m23 m33 |
		//
		// Or, using column vector notation:
		//
		// Inertial to object:
		//
		//     | m11 m21 m31 | | ix |	| ox |
		//     | m12 m22 m32 | | iy | = | oy |
		//     | m13 m23 m33 | | iz |	| oz |
		//
		// Object to inertial:
		//
		//     | m11 m12 m13 | | ox |	| ix |
		//     | m21 m22 m23 | | oy | = | iy |
		//     | m31 m32 m33 | | oz |	| iz |
		//*!

		#region public members
		public float	m11, m12, m13;
		public float	m21, m22, m23;
		public float	m31, m32, m33;
		#endregion
		
		#region methods
		
		/// <summary>
		/// Set to identity
		/// </summary>
		public void	Identity()
		{
			m11 = 1.0f; m12 = 0.0f; m13 = 0.0f;
			m21 = 0.0f; m22 = 1.0f; m23 = 0.0f;
			m31 = 0.0f; m32 = 0.0f; m33 = 1.0f;
		}
		
		public void	Setup(EulerAngles orientation)
		{
			// Fetch sine and cosine of angles
			float sp = 0;
			float sb = 0;
			float sh = 0;
			float cp = 0;
			float cb = 0;
			float ch = 0;
			mathUtil.SinCos(ref sh, ref ch, orientation.heading);
			mathUtil.SinCos(ref sp, ref cp, orientation.pitch);
			mathUtil.SinCos(ref sb, ref cb, orientation.bank);
		
			// Fill in the matrix elements
		
			m11 = ch * cb + sh * sp * sb;
			m12 = -ch * sb + sh * sp * cb;
			m13 = sh * cp;
		
			m21 = sb * cp;
			m22 = cb * cp;
			m23 = -sp;
		
			m31 = -sh * cb + ch * sp * sb;
			m32 = sb * sh + ch * sp * cb;
			m33 = ch * cp;
		}
		
		/// <summary>
		/// Setup the matrix from a quaternion, assuming the
		/// quaternion performs the rotation in the
		/// specified direction of transformation
		/// </summary>
		/// <param name="q"></param>
		public void	FromInertialToObjectQuaternion(Quaternion q)
		{
			// Fill in the matrix elements.  This could possibly be
			// optimized since there are many common subexpressions.
			// We'll leave that up to the compiler...
		
			m11 = 1.0f - 2.0f * (q.y*q.y + q.z*q.z);
			m12 = 2.0f * (q.x*q.y + q.w*q.z);
			m13 = 2.0f * (q.x*q.z - q.w*q.y);
		
			m21 = 2.0f * (q.x*q.y - q.w*q.z);
			m22 = 1.0f - 2.0f * (q.x*q.x + q.z*q.z);
			m23 = 2.0f * (q.y*q.z + q.w*q.x);
		
			m31 = 2.0f * (q.x*q.z + q.w*q.y);
			m32 = 2.0f * (q.y*q.z - q.w*q.x);
			m33 = 1.0f - 2.0f * (q.x*q.x + q.y*q.y);
		}
		
		public void	FromObjectToInertialQuaternion(Quaternion q)
		{
			// Fill in the matrix elements.  This could possibly be
			// optimized since there are many common subexpressions.
			// We'll leave that up to the compiler...
		
			m11 = 1.0f - 2.0f * (q.y*q.y + q.z*q.z);
			m12 = 2.0f * (q.x*q.y - q.w*q.z);
			m13 = 2.0f * (q.x*q.z + q.w*q.y);
		
			m21 = 2.0f * (q.x*q.y + q.w*q.z);
			m22 = 1.0f - 2.0f * (q.x*q.x + q.z*q.z);
			m23 = 2.0f * (q.y*q.z - q.w*q.x);
		
			m31 = 2.0f * (q.x*q.z - q.w*q.y);
			m32 = 2.0f * (q.y*q.z + q.w*q.x);
			m33 = 1.0f - 2.0f * (q.x*q.x + q.y*q.y);
		}
		
		/// <summary>
		/// Perform rotations
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public Vector3	InertialToObject(Vector3 v)
		{
			// Perform the matrix multiplication in the "standard" way.
			return new Vector3(
				m11*v.x + m21*v.y + m31*v.z,
				m12*v.x + m22*v.y + m32*v.z,
				m13*v.x + m23*v.y + m33*v.z
			);
		}
		
		public Vector3	ObjectToInertial(Vector3 v)
		{
			// Multiply by the transpose
			return new Vector3(
				m11*v.x + m12*v.y + m13*v.z,
				m21*v.x + m22*v.y + m23*v.z,
				m31*v.x + m32*v.y + m33*v.z
			);

		}
		
		#endregion
	}

	/// <summary>
	/// Quaternion class.
	/// </summary>
	public class Quaternion
	{
		////////////////////////////////////////////////////////////////////////////
		//
		// 3D Math Primer for Games and Graphics Development
		//
		// Quaternion.cpp - Quaternion implementation
		//
		// Visit gamemath.com for the latest version of this file.
		//
		// For more details see section 11.3.
		//
		/////////////////////////////////////////////////////////////////////////////
		
		public float x, y, z, w;
		
		// The global identity quaternion.  Notice that there are no constructors
		// to the Quaternion class, since we really don't need any.
		
		public void SetIdentity()
		{
			w = 1.0f;
			x = 0.0f;
			y = 0.0f;
			z = 0.0f;
		}
		
		#region members
		MathUtil mathUtil = new MathUtil();
		Vector3 vector3 = new Vector3();
		#endregion
		
		#region member functions
		
		/// <summary>
		/// Setup the quaternion to rotate about the X axis
		/// </summary>
		/// <param name="theta">amount to rotate, in degrees</param>
		public void SetToRotateAboutX(float theta) 
		{
			// Compute the half angle
			float thetaOver2 = theta * .5f;
		
			// Set the values
			w = (float)System.Math.Cos(mathUtil.DegToRad(thetaOver2));
			x = (float)System.Math.Sin(mathUtil.DegToRad(thetaOver2));
			y = 0.0f;
			z = 0.0f;
		}
		
		/// <summary>
		/// Setup the quaternion to rotate about the Y axis
		/// </summary>
		/// <param name="theta">amount to rotate, in degrees</param>
		public void	SetToRotateAboutY(float theta) 
		{
			// Compute the half angle
			float	thetaOver2 = theta * .5f;
		
			// Set the values
			w = (float)System.Math.Cos(mathUtil.DegToRad(thetaOver2));
			x = 0.0f;
			y = (float)System.Math.Sin(mathUtil.DegToRad(thetaOver2));
			z = 0.0f;
		}
		
		/// <summary>
		/// Setup the quaternion to rotate about the Z axis
		/// </summary>
		/// <param name="theta">amount to rotate, in degrees</param>
		public void SetToRotateAboutZ(float theta) 
		{
			// Compute the half angle
			float	thetaOver2 = theta * .5f;
		
			// Set the values
			w = (float)System.Math.Cos(mathUtil.DegToRad(thetaOver2));
			x = 0.0f;
			y = 0.0f;
			z = (float)System.Math.Sin(mathUtil.DegToRad(thetaOver2));
		}
		
		/// <summary>
		/// Setup the quaternion to rotate about an arbitrary axis
		/// </summary>
		/// <param name="theta">amount to rotate, in degrees</param>
		public void	SetToRotateAboutAxis(Vector3 axis, float theta) 
		{
			// The axis of rotation must be normalized
			Debug.Assert(System.Math.Abs(vector3.VectorMag(axis) - 1.0f) < .01f);
		
			// Compute the half angle and its sin
			float	thetaOver2 = theta * .5f;
			float	sinThetaOver2 = (float)System.Math.Sin(mathUtil.DegToRad(thetaOver2));
		
			// Set the values
			w = (float)System.Math.Cos(mathUtil.DegToRad(thetaOver2));
			x = axis.x * sinThetaOver2;
			y = axis.y * sinThetaOver2;
			z = axis.z * sinThetaOver2;
		}
		
		/// <summary>
		/// Setup the quaternion to perform an object->inertial rotation, given the
		/// orientation in Euler angle format
		///
		/// See 10.6.5 for more information.
		/// </summary>
		/// <param name="orientation"></param>
		public void	SetToRotateObjectToInertial(EulerAngles orientation) 
		{
			float sp = 0;
			float sb = 0;
			float sh = 0;
			float cp = 0;
			float cb = 0;
			float ch = 0;
			
			// Compute sine and cosine of the half angles
			mathUtil.SinCos(ref sp, ref cp, orientation.pitch * 0.5f);
			mathUtil.SinCos(ref sb, ref cb, orientation.bank * 0.5f);
			mathUtil.SinCos(ref sh, ref ch, orientation.heading * 0.5f);
		
			// Compute values
			w =  ch*cp*cb + sh*sp*sb;
			x =  ch*sp*cb + sh*cp*sb;
			y = -ch*sp*sb + sh*cp*cb;
			z = -sh*sp*cb + ch*cp*sb;
		
		}
		
		/// <summary>
		/// Setup the quaternion to perform an inertial->object rotation, given the
		/// orientation in Euler angle format
		///
		/// See 10.6.5 for more information.
		/// </summary>
		/// <param name="orientation"></param>
		public void SetToRotateInertialToObject(EulerAngles orientation) 
		{
			float sp = 0;
			float sb = 0;
			float sh = 0;
			float cp = 0;
			float cb = 0;
			float ch = 0;
			
			// Compute sine and cosine of the half angles
			mathUtil.SinCos(ref sp, ref cp, orientation.pitch * 0.5f);
			mathUtil.SinCos(ref sb, ref cb, orientation.bank * 0.5f);
			mathUtil.SinCos(ref sh, ref ch, orientation.heading * 0.5f);
		
			// Compute values
			w =  ch*cp*cb + sh*sp*sb;
			x = -ch*sp*cb - sh*cp*sb;
			y =  ch*sp*sb - sh*cb*cp;
			z =  sh*sp*cb - ch*cp*sb;
		}
		
		//!
		// Quaternion::operator *
		//
		// *!
		
		/// <summary>
		/// Quaternion cross product, which concatonates multiple angular displacements.
		/// 
		/// The order of multiplication, from left to right, corresponds to the order 
		/// that the angular displacements are applied.
		/// 
		/// This is backwards from the *standard* definition of quaternion multiplication.  
		/// 
		/// See section 10.4.8 for the rationale behind this deviation from the standard.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public Quaternion CrossProduct(Quaternion a)
		{
			Quaternion result = new Quaternion();
		
			result.w = w*a.w - x*a.x - y*a.y - z*a.z;
			result.x = w*a.x + x*a.w + z*a.y - y*a.z;
			result.y = w*a.y + y*a.w + x*a.z - z*a.x;
			result.z = w*a.z + z*a.w + y*a.x - x*a.y;
		
			return result;
		}
		
		//!
		// Quaternion::operator *=
		//
		// Combined cross product and assignment, as per C++ convention *!
		
//		Quaternion operator *=(const Quaternion &a) 
//		{
//		
//			// Multuiply and assign
//		
//			*this = *this * a;
//		
//			// Return reference to l-value
//		
//			return *this;
//		}
		
		/// <summary>
		/// "Normalize" a quaternion.  Note that normally, quaternions
		/// are always normalized (within limits of numerical precision).
		/// See section 10.4.6 for more information.
		///
		/// This function is provided primarily to combat floating point "error
		/// creep," which can occur when many successive quaternion operations
		/// are applied.
		/// </summary>
		public void Normalize() 
		{
			// Compute magnitude of the quaternion
			float	mag = (float)System.Math.Sqrt(w*w + x*x + y*y + z*z);
		
			// Check for bogus length, to protect against divide by zero
			if (mag > 0.0f) {
		
				// Normalize it
				float	oneOverMag = 1.0f / mag;
				w *= oneOverMag;
				x *= oneOverMag;
				y *= oneOverMag;
				z *= oneOverMag;
		
			} else {
		
				// Houston, we have a problem
		
				Debug.Assert(false);
		
				// In a release build, just slam it to something
		
				SetIdentity();
			}
		}
		
		/// <summary>
		/// Return the rotation angle theta
		/// </summary>
		/// <returns>the rotation angle theta</returns>
		public float GetRotationAngle()
		{
			// Compute the half angle.  Remember that w = cos(theta / 2)
			float thetaOver2 = mathUtil.SafeAcos(w);
		
			// Return the rotation angle
			return thetaOver2 * 2.0f;
		}
		
		/// <summary>
		/// Return the rotation axis
		/// </summary>
		/// <returns>the rotation axis</returns>
		public Vector3	GetRotationAxis()
		{
			// Compute sin^2(theta/2).  Remember that w = cos(theta/2),
			// and sin^2(x) + cos^2(x) = 1
			float sinThetaOver2Sq = 1.0f - w*w;
		
			// Protect against numerical imprecision
			if (sinThetaOver2Sq <= 0.0f) {
		
				// Identity quaternion, or numerical imprecision.  Just
				// return any valid vector, since it doesn't matter
		
				return new Vector3(1.0f, 0.0f, 0.0f);
			}
		
			// Compute 1 / sin(theta/2
			float	oneOverSinThetaOver2 = 1.0f / (float)System.Math.Sqrt(sinThetaOver2Sq);
		
			// Return axis of rotation
			return new Vector3(
				x * oneOverSinThetaOver2,
				y * oneOverSinThetaOver2,
				z * oneOverSinThetaOver2
			);
		}
		
		#endregion
		
		#region non-member functions
		
		/// <summary>
		/// Quaternion dot product.  We use a nonmember function so we can
		/// pass quaternion expressions as operands without having "funky syntax"
		///
		/// See 10.4.10
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public float DotProduct(Quaternion a, Quaternion b) 
		{
			return a.w*b.w + a.x*b.x + a.y*b.y + a.z*b.z;
		}
		
		/// <summary>
		/// Spherical linear interpolation.
		/// 
		/// See 10.4.13
		/// </summary>
		/// <param name="q0"></param>
		/// <param name="q1"></param>
		/// <param name="t"></param>
		/// <returns>quaternion</returns>
		public Quaternion Slerp(Quaternion q0, Quaternion q1, float t) 
		{
			// Check for out-of range parameter and return edge points if so
		
			if (t <= 0.0f) return q0;
			if (t >= 1.0f) return q1;
		
			// Compute "cosine of angle between quaternions" using dot product
		
			float cosOmega = DotProduct(q0, q1);
		
			// If negative dot, use -q1.  Two quaternions q and -q
			// represent the same rotation, but may produce
			// different slerp.  We chose q or -q to rotate using
			// the acute angle.
		
			float q1w = q1.w;
			float q1x = q1.x;
			float q1y = q1.y;
			float q1z = q1.z;
			if (cosOmega < 0.0f) {
				q1w = -q1w;
				q1x = -q1x;
				q1y = -q1y;
				q1z = -q1z;
				cosOmega = -cosOmega;
			}
		
			// We should have two unit quaternions, so dot should be <= 1.0
		
			Debug.Assert(cosOmega < 1.1f);
		
			// Compute interpolation fraction, checking for quaternions
			// almost exactly the same
		
			float k0, k1;
			if (cosOmega > 0.9999f) {
		
				// Very close - just use linear interpolation,
				// which will protect againt a divide by zero
		
				k0 = 1.0f-t;
				k1 = t;
		
			} else {
		
				// Compute the sin of the angle using the
				// trig identity sin^2(omega) + cos^2(omega) = 1
		
				float sinOmega = (float)System.Math.Sqrt(1.0f - cosOmega*cosOmega);
		
				// Compute the angle from its sin and cosine
		
				float omega = (float)System.Math.Atan2(mathUtil.DegToRad(sinOmega), mathUtil.DegToRad(cosOmega));
		
				// Compute inverse of denominator, so we only have
				// to divide once
		
				float oneOverSinOmega = 1.0f / sinOmega;
		
				// Compute interpolation parameters
		
				k0 = (float)(System.Math.Sin(mathUtil.DegToRad((1.0f - t) * omega)) * oneOverSinOmega);
				k1 = (float)(System.Math.Sin(mathUtil.DegToRad(t * omega) * oneOverSinOmega));
			}
		
			// Interpolate
		
			Quaternion result = new Quaternion();
			result.x = k0*q0.x + k1*q1x;
			result.y = k0*q0.y + k1*q1y;
			result.z = k0*q0.z + k1*q1z;
			result.w = k0*q0.w + k1*q1w;
		
			// Return it
		
			return result;
		}
		
		/// <summary>
		/// Compute the quaternion conjugate.  
		/// This is the quaternian with the opposite rotation as the original quaternian.  
		/// 
		/// See 10.4.7
		/// </summary>
		/// <param name="q"></param>
		/// <returns></returns>
		public Quaternion Conjugate(Quaternion q) 
		{
			Quaternion result = new Quaternion();
		
			// Same rotation amount
			result.w = q.w;
		
			// Opposite axis of rotation
			result.x = -q.x;
			result.y = -q.y;
			result.z = -q.z;
		
			// Return it
			return result;
		}
		
		/// <summary>
		/// Quaternion exponentiation.
		/// 
		/// See 10.4.12
		/// </summary>
		/// <param name="q">quaternion</param>
		/// <param name="exponent">exponent</param>
		/// <returns></returns>
		public Quaternion Pow(Quaternion q, float exponent) 
		{
			// Check for the case of an identity quaternion.
			// This will protect against divide by zero
		
			if (System.Math.Abs(q.w) > .9999f) {
				return q;
			}
	
			// Extract the half angle alpha (alpha = theta/2)
			float	alpha = (float)System.Math.Acos(mathUtil.DegToRad((q.w)));
		
			// Compute new alpha value
			float	newAlpha = alpha * exponent;
		
			// container to hold the resulting quaternion
			Quaternion result = new Quaternion();
			
			// Compute new w value
			result.w = (float)(System.Math.Cos(mathUtil.DegToRad(newAlpha)));
		
			// Compute new xyz values
			float	mult = (float)(System.Math.Sin(mathUtil.DegToRad(newAlpha)) / System.Math.Sin(mathUtil.DegToRad(alpha)));
			result.x = q.x * mult;
			result.y = q.y * mult;
			result.z = q.z * mult;
		
			// Return it
			return result;
		}

		#endregion
	}
	
	/// <summary>
	/// Euler angles class. 
	/// Used to describe rotations in heading, pitch, and bank.
	/// </summary>
	public class EulerAngles
	{
		//!Straightforward representation.  Store the three angles, in radians.
		public float	heading;
		public float	pitch;
		public float	bank;

		MathUtil mathUtil = new MathUtil();
		
		/// <summary>
		/// default constructor
		/// </summary>
		public EulerAngles() {}

		/// <summary>
		/// construct from three angles
		/// </summary>
		/// <param name="h"></param>
		/// <param name="p"></param>
		/// <param name="b"></param>
		public EulerAngles(float h, float p, float b)
		{
			heading = h;
			pitch = p;
			bank = b;	
		}
		
		/// <summary>
		/// Set the pitch, bank, and heading values to 0.
		/// </summary>
		public void	Identity() 
		{ 
			pitch = bank = heading = 0.0f; 
		}
		
		/// <summary>
		/// Determine "canonical" Euler angle triple.
		/// </summary>
		public void	Canonize()
		{
			// First, wrap pitch in range -pi ... pi
			pitch = mathUtil.WrapPi(pitch);

			// Now, check for "the back side" of the matrix, pitch outside
			// the canonical range of -pi/2 ... pi/2
		
			if (pitch < -mathUtil.kPiOver2) {
				pitch = -mathUtil.kPi - pitch;
				heading += mathUtil.kPi;
				bank += mathUtil.kPi;
			} else if (pitch > mathUtil.kPiOver2) {
				pitch = mathUtil.kPi - pitch;
				heading += mathUtil.kPi;
				bank += mathUtil.kPi;
			}
		
			// OK, now check for the gimbel lock case (within a slight
			// tolerance)
		
			if (System.Math.Abs(pitch) > mathUtil.kPiOver2 - 1e-4) {
		
				// We are in gimbel lock.  Assign all rotation
				// about the vertical axis to heading
				heading += bank;
				bank = 0.0f;
		
			} else {
		
				// Not in gimbel lock.  Wrap the bank angle in canonical range 
				bank = mathUtil.WrapPi(bank);
			}
		
			// Wrap heading in canonical range
			heading = mathUtil.WrapPi(heading);
		}
		
		/// <summary>
		/// Convert the quaternion to Euler angle format.  
		/// 
		/// The input quaternion is assumed to perform the rotation from object-to-inertial
		/// or inertial-to-object, as indicated.
		/// </summary>
		/// <param name="q">quaternion</param>
		public void	FromObjectToInertialQuaternion(ref Quaternion q)
		{
			// Extract sin(pitch)
			float sp = -2.0f * (q.y*q.z - q.w*q.x);
		
			// Check for Gimbel lock, giving slight tolerance for numerical imprecision
			if (System.Math.Abs(sp) > 0.9999f) 
			{
				// Looking straight up or down
				pitch = mathUtil.kPiOver2 * sp;
		
				// Compute heading, slam bank to zero
				heading = (float)System.Math.Atan2(-q.x*q.z + q.w*q.y,
				                     0.5f - q.y*q.y - q.z*q.z);
				bank = 0.0f;
			} 
			else 
			{
				// Compute angles.  We don't have to use the "safe" asin
				// function because we already checked for range errors when
				// checking for Gimbel lock
		
				pitch	= (float)System.Math.Asin(sp);
				heading	= (float)System.Math.Atan2(q.x*q.z + q.w*q.y, 0.5f - q.x*q.x - q.y*q.y);
				bank	= (float)System.Math.Atan2(q.x*q.y + q.w*q.z, 0.5f - q.x*q.x - q.z*q.z);
			}
		}
		
		/// <summary>
		/// Setup the Euler angles, given an inertial->object rotation quaternion.
		/// </summary>
		/// <param name="q"></param>
		public void	FromInertialToObjectQuaternion(ref Quaternion q)
		{
			// Extract sin(pitch)

			float sp = -2.0f * (q.y*q.z + q.w*q.x);
		
			// Check for Gimbel lock, giving slight tolerance for numerical imprecision
		
			if (System.Math.Abs(sp) > 0.9999f) {
		
				// Looking straight up or down
		
				pitch = mathUtil.kPiOver2 * sp;
		
				// Compute heading, slam bank to zero
		
				heading = (float)System.Math.Atan2(-q.x*q.z - q.w*q.y, 0.5f - q.y*q.y - q.z*q.z);
				bank = 0.0f;
		
			} 
			else 
			{
		
				// Compute angles.  We don't have to use the "safe" asin
				// function because we already checked for range errors when
				// checking for Gimbel lock
		
				pitch	= (float)System.Math.Asin(sp);
				heading	= (float)System.Math.Atan2(q.x*q.z - q.w*q.y, 0.5f - q.x*q.x - q.y*q.y);
				bank	= (float)System.Math.Atan2(q.x*q.y - q.w*q.z, 0.5f - q.x*q.x - q.z*q.z);
			}
		}
		
		//!*Convert the transform matrix to Euler angle format.  The input
		// matrix is assumed to perform the transformation from
		// object-to-world, or world-to-object, as indicated.  The
		// translation portion of the matrix is ignored.  The
		// matrix is assumed to be orthogonal.*!
		
		/// <summary>
		/// Setup the Euler angles, given an object->world transformation matrix.
		/// </summary>
		/// <param name="m"></param>
		public void	FromObjectToWorldMatrix(ref Matrix4x3 m)
		{
			// Extract sin(pitch) from m32.
			float	sp = -m.m32;
		
			// Check for Gimbel lock
			if (System.Math.Abs(sp) > 9.99999f) {
		
				// Looking straight up or down
				pitch = mathUtil.kPiOver2 * sp;
		
				// Compute heading, slam bank to zero
				heading = (float)System.Math.Atan2(-m.m23, m.m11);
				bank = 0.0f;
		
			} else {
		
				// Compute angles.  We don't have to use the "safe" asin
				// function because we already checked for range errors when
				// checking for Gimbel lock
		
				heading = (float)System.Math.Atan2(m.m31, m.m33);
				pitch = (float)System.Math.Asin(sp);
				bank = (float)System.Math.Atan2(m.m12, m.m22);
			}
		}
		
		/// <summary>
		/// Setup the Euler angles, given a world->object transformation matrix.
		/// </summary>
		/// <param name="m"></param>
		public void	FromWorldToObjectMatrix(ref Matrix4x3 m)
		{
			// Extract sin(pitch) from m23.
			float	sp = -m.m23;
		
			// Check for Gimbel lock
			
			if (System.Math.Abs(sp) > 9.99999f) {
		
				// Looking straight up or down
				pitch = mathUtil.kPiOver2 * sp;
		
				// Compute heading, slam bank to zero
				heading = (float)System.Math.Atan2(-m.m31, m.m11);
				bank = 0.0f;
		
			} else {
		
				// Compute angles.  We don't have to use the "safe" asin
				// function because we already checked for range errors when
				// checking for Gimbel lock
		
				heading = (float)System.Math.Atan2(m.m13, m.m33);
				pitch = (float)System.Math.Asin(sp);
				bank = (float)System.Math.Atan2(m.m21, m.m22);
			}
		}
		
		/// <summary>
		/// Convert a rotation matrix to Euler Angle form.
		/// </summary>
		/// <param name="m"></param>
		public void	FromRotationMatrix(ref RotationMatrix m)
		{
			// Extract sin(pitch) from m23.
			float	sp = -m.m23;
		
			// Check for Gimbel lock
			if (System.Math.Abs(sp) > 9.99999f) {
		
				// Looking straight up or down
				pitch = mathUtil.kPiOver2 * sp;
		
				// Compute heading, slam bank to zero
				heading = (float)System.Math.Atan2(-m.m31, m.m11);
				bank = 0.0f;
		
			} else {
		
				// Compute angles.  We don't have to use the "safe" asin
				// function because we already checked for range errors when
				// checking for Gimbel lock
		
				heading = (float)System.Math.Atan2(m.m13, m.m33);
				pitch = (float)System.Math.Asin(sp);
				bank = (float)System.Math.Atan2(m.m21, m.m22);
			}
		}
	}
	
	/// <summary>
	/// Math Utility functions
	/// </summary>
	public class MathUtil
	{
		public float kPi;
		public float k2Pi;
		public float kPiOver2;
		public float k1OverPi;
		public float k1Over2Pi;
		public float kPiOver180;
		public float k180OverPi;
		
		public Vector3 kZeroVector;
		
		public MathUtil()
		{
			kPi = 3.14159265f;
			k2Pi = kPi * 2.0f;
			kPiOver2 = kPi / 2.0f;
			k1OverPi = 1.0f / kPi;
			k1Over2Pi = 1.0f / k2Pi;
			kPiOver180 = kPi / 180.0f;
			k180OverPi = 180.0f / kPi;
			kZeroVector = new Vector3(0.0f, 0.0f, 0.0f);
		}
		
		#region distance related methods
		public static double FeetToMilli(double feet) { return feet * 304.8; }
		
		#endregion
		
		#region angle related methods
		
		/// <summary>
		/// "Wrap" an angle in range -pi...pi by adding the correct multiple of 2 pi.
		/// </summary>
		/// <param name="theta">angle to be wrapped</param>
		/// <returns></returns>
		public float WrapPi(float theta)
		{
			theta += kPi;
			theta -= (float)System.Math.Floor(theta * k1Over2Pi) * k2Pi;
			theta -= kPi;
			return theta;
		}
		
		/// <summary>
		/// Same as acos(x), but if x is out of range, it is "clamped" to the nearest
		/// valid value.  The value returned is in range 0...pi, the same as the
		/// standard C acos() function
		/// </summary>
		/// <param name="x">angle in degrees</param>
		/// <returns></returns>
		public float SafeAcos(float x)
		{
			// Check limit conditions
			if (x <= -1.0f) {
				return kPi;
			}
			if (x >= 1.0f) {
				return 0.0f;
			}
		
			// Value is in the domain - use standard C function
			return (float)System.Math.Acos((double)this.DegToRad(x));
		}
	
		/// <summary>
		/// Convert degrees to radians.
		/// </summary>
		/// <param name="deg">angle in degrees</param>
		/// <returns></returns>
		public float DegToRad(float deg) { return deg * kPiOver180; }
		
		/// <summary>
		/// convert radians to degrees
		/// </summary>
		/// <param name="rad">angle in radians</param>
		/// <returns></returns>
		public float RadToDeg(float rad) { return rad * k180OverPi; }
		
		/// <summary>
		/// Compute the sin and cosine of an angle.
		/// On some platforms, if we know that we need both values, it can be computed faster than computing
		/// the two values seperately.
		/// </summary>
		/// <param name="returnSin">placeholder for the sin value</param>
		/// <param name="returnCos">placeholder for the cos value</param>
		/// <param name="theta">angle to be computed, in degrees</param>
		public void SinCos(ref float returnSin, ref float returnCos, float theta) 
		{
			// For simplicity, we'll just use the normal trig functions.
			// Note that on some platforms we may be able to do better
	
			returnSin = (float)System.Math.Sin((double)this.DegToRad(theta));
			returnCos = (float)System.Math.Cos((double)this.DegToRad(theta));
		}
		
		#endregion
		
		public float FovToZoom(float fov) { return 1.0f /(float) System.Math.Tan(fov * .5f); }
		
		public float ZoomToFov(float zoom) { return 2.0f * (float)System.Math.Atan(1.0f / zoom); }
		
		public static int FindWithinListByTolerance(ArrayList values, double tolerance, double input)
        {
        	for (int i=0; i< values.Count; i++)
        	{
        		double b = (double) values[i];
        		if (input >= b-tolerance && input <= b + tolerance)
        		{
        			return i;
        		}
        	}
        	return -1;
        } 
	}
}
