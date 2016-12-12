using System.Collections.Generic;

namespace NReco.CF.Taste.Model 
{
	/// <summary>
	/// An alternate representation of an array of {@link Preference}.
	/// Implementations, in theory, can produce a more memory-efficient representation.
	/// </summary>
	public interface IPreferenceArray : IEnumerable<IPreference> 
    {
		/// <summary>
		/// Size of length of the "array"
		/// </summary>
		int Length();

		/// <summary>
		/// Get preference at specified index
		/// </summary>
		/// <param name="i">index</param>
		/// <returns>a materialized <see cref="IPreference"/> representation of the preference at i</returns>
		IPreference Get(int i);

		/// <summary>
		/// Sets preference at i from information in the given <see cref="IPreference"/>
		/// </summary>
		/// <param name="i">index</param>
		/// <param name="pref">pref</param>
		void Set(int i, IPreference pref);

		/// <summary>
		/// Get user ID from preference at specified index
		/// </summary>
		/// <param name="i">index</param>
		/// <returns>user ID from preference at i</returns>
		long GetUserID(int i);
  
		/// Sets user ID for preference at i.
		/// 
		/// @param i
		///          index
		/// @param userID
		///          new user ID
		void SetUserID(int i, long userID);

		/// <summary>
		/// Get item ID from preference at specified index
		/// </summary>
		/// <param name="i">index</param>
		/// <returns>item ID from preference at i</returns>
		long GetItemID(int i);

		/// <summary>
		/// Sets item ID for preference at i.
		/// </summary>
		/// <param name="i">index</param>
		/// <param name="itemID">new item ID</param>
		void SetItemID(int i, long itemID);

		/// <summary>
		/// Get all IDs
		/// </summary>
		/// <returns>all user or item IDs</returns>
		long[] GetIDs();
  
		/// <summary>
		/// Get preference value
		/// </summary>
		/// <param name="i">index</param>
		/// <returns>preference value from preference at i</returns>
		float GetValue(int i);

		/// <summary>
		/// Sets preference value for preference at i.
		/// </summary>
		/// <param name="i">index</param>
		/// <param name="value">new preference value</param>
		void SetValue(int i, float value);
  
		/// <summary>
		/// Clone object instance
		/// </summary>
		/// <returns>independent copy of this object</returns>
		IPreferenceArray Clone();
  
		/// <summary>
		/// Sorts underlying array by user ID, ascending.
		/// </summary>
		void SortByUser();
  
		/// <summary>
		/// Sorts underlying array by item ID, ascending.
		/// </summary>
		void SortByItem();
  
		/// <summary>
		/// Sorts underlying array by preference value, ascending.
		/// </summary>
		void SortByValue();
  
		/// <summary>
		/// Sorts underlying array by preference value, descending.
		/// </summary>
		void SortByValueReversed();

		/// <summary>
		/// Check if array contains a preference with given user ID
		/// </summary>
		/// <param name="userID">user ID</param>
		/// <returns>true if array contains a preference with given user ID</returns>
		bool HasPrefWithUserID(long userID);

		/// <summary>
		/// Check if array contains a preference with given item ID
		/// </summary>
		/// <param name="itemID">item ID</param>
		/// <returns>true if array contains a preference with given item ID</returns>
		bool HasPrefWithItemID(long itemID);
	}
}