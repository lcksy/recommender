
namespace NReco.CF.Taste.Model 
{
	/// <summary>
	/// A <see cref="IPreference"/> encapsulates an item and a preference value, which indicates the strength of the
	/// preference for it. <see cref="IPreference"/>s are associated to users.
	/// </summary>
	public interface IPreference 
    {
		/// <summary>
		/// ID of user who prefers the item 
		/// </summary>
		long GetUserID();
  
		/// <summary>
		/// Item ID that is preferred 
		/// </summary>
		long GetItemID();

		/// <summary>
		/// Strength of the preference for that item. 
		/// </summary>
		/// <remarks>Zero should indicate "no preference either way"; positive values indicate preference and negative values indicate dislike.</remarks>
		float GetValue();

		/// <summary>
		/// Sets the strength of the preference for this item
		/// </summary>
		/// <param name="value">new preference</param>
		void SetValue(float value);
	}
}