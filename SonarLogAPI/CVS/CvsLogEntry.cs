﻿namespace SonarLogAPI.CVS
{
	using System;
	using System.Collections.Generic;

	using Primitives;

	/// <summary>
	/// The log entry, consists of Latitude, Longitude and Depth, separated by comma
	/// </summary>
	public class CvsLogEntry : IDepthPointSource, IEquatable<CvsLogEntry>
	{
		/// <summary>
		/// Represents a geographical location point that is determined by <see cref="SonarLogAPI.Primitives.Latitude" /> and <see cref="SonarLogAPI.Primitives.Longitude" /> coordinates.
		/// </summary>
		public CoordinatePoint Point { get; set; }

		/// <summary>
		/// Water depth at point
		/// </summary>
		public LinearDimension Depth { get; set; }

		/// <summary>
		/// Log entry values, expect <see cref="SonarLogAPI.Primitives.Latitude" />, <see cref="SonarLogAPI.Primitives.Longitude" /> and Depth
		/// </summary>
		public List<string> UnexpectedValues { get; set; }
		
		/// <summary>
		/// CvsLogEntry default constructor
		/// </summary>
		public CvsLogEntry() { }

		/// <summary>
		/// Creates CvsLogEntry from <see cref="IDepthPointSource"/> instance.
		/// </summary>
		public CvsLogEntry(IDepthPointSource source)
		{
			Point = source.Point;
			Depth = source.Depth;
		}

		/// <summary>
		/// Create instance of <see cref="CvsLogEntry" /> object from objects of <see cref="CoordinatePoint" /> add <see cref="LinearDimension" /> double value
		/// </summary>
		/// <param name="point"><see cref="CoordinatePoint" /> object.</param>
		/// <param name="depth">Depth double value</param>
		public CvsLogEntry(CoordinatePoint point, LinearDimension depth)
		{
			Point = point;
			Depth = depth;
		}


		/// <summary>
		/// Create instance of <see cref="CvsLogEntry" /> object from objects of <see cref="Latitude" />, <see cref="Longitude" /> add <see cref="LinearDimension" /> double value
		/// </summary>
		/// <param name="latitude">Latitude</param>
		/// <param name="longitude">Longitude</param>
		/// <param name="depth">Depth double value</param>
		public CvsLogEntry(Latitude latitude, Longitude longitude, LinearDimension depth)
		{
			Point = new CoordinatePoint(latitude, longitude);
			Depth = depth;
		}

		/// <summary>
		/// Create instance of CvsLogEntry object from double values of Latitude, Longitude add Depth
		/// </summary>
		/// <param name="latitude">Latitude degrees double value</param>
		/// <param name="longitude">Longitude degrees double value</param>
		/// <param name="depth">Depth double value</param>
		/// <param name="depthUnit">Depth value unit</param>
		public CvsLogEntry(double latitude, double longitude, double depth, LinearDimensionUnit depthUnit) 
			: this(new Latitude(latitude), new Longitude(longitude), new LinearDimension(depth, depthUnit)) { }

		/// <summary>
		/// TryParse CVS log string, consists of Latitude, Longitude, Depth(and may be other values), separated by comma
		/// </summary>
		/// <param name="cvsLogEntryString">String to parse</param>
		/// <param name="charForSplit">Char for sting split</param>
		/// <param name="depthUnit">Depth unit</param>
		/// <param name="valuesOrder">Represent order of CvsLogEntry properties in string</param>
		/// <param name="result">CvsLogEntry</param>
		/// <returns>Conversion successed or failed</returns>
		public static bool TryParse(string cvsLogEntryString, char charForSplit, LinearDimensionUnit depthUnit, Dictionary<int,string> valuesOrder, out CvsLogEntry result)
		{
			result = new CvsLogEntry { UnexpectedValues = new List<string>() };

			Latitude lat = null;
			Longitude lon = null;
			LinearDimension dpt = null;
			var values = cvsLogEntryString.Split(charForSplit);
			
			// if string does't contanes values or values count less then expect return false
			if (values.Length == 0 || values.Length < valuesOrder.Count)
				return false;

			for (var i = 0; i < values.Length; i++)
			{
				if (i < valuesOrder.Count)
				{
					if (valuesOrder[i].Contains("Latitude"))
					{
						var parceresult = Latitude.TryParse(values[i], out lat);
						if (!parceresult)
							return false;
					}

					if (valuesOrder[i].Contains("Longitude"))
					{
						var parceresult = Longitude.TryParse(values[i], out lon);
						if (!parceresult)
							return false;
					}

					if (valuesOrder[i].Contains("Depth"))
					{
						var parceresult = LinearDimension.TryParse(values[i], depthUnit, out dpt);
						if (!parceresult)
							return false;
					}
				}
				else
				{
					result.UnexpectedValues.Add(values[i]);
				}
			}

			if (lat != null && lon != null && dpt != null)
			{
				result.Point = new CoordinatePoint(lat, lon);
				result.Depth = dpt;
			}
			else
				return false;

			return true;
		}

		public bool Equals(CvsLogEntry other)
		{
			//Check whether the compared object is null. 
			if (ReferenceEquals(other, null)) return false;

			//Check whether the compared object references the same data. 
			if (ReferenceEquals(this, other)) return true;

			//Check whether the CvsLogEntry' properties are equal. 
			return Depth.Equals(other.Depth) && Point.Equals(other.Point);
		}

		public override bool Equals(object obj)
		{
			var item = obj as CvsLogEntry;

			return item != null && Equals(item);
		}

		public static bool operator ==(CvsLogEntry left, CvsLogEntry right)
		{
			return (object)left != null && (object)right != null && left.Equals(right);
		}

		public static bool operator !=(CvsLogEntry left, CvsLogEntry right)
		{
			return !(left == right);
		}

		public override int GetHashCode()
		{
			return (Depth?.GetHashCode() ?? 0) ^ (Point?.GetHashCode() ?? 0);
		}

		public override string ToString()
		{
			return $"{Point},{Depth.GetMeters()}";
		}

	}
}
