using System.Collections;
using System.Collections.Generic;

namespace CustomDataTypes
{
	public class OrientationCueData : IEnumerable<string>
	{
		public string NorthName { get; set; }
		public string SouthName { get; set; }
		public string EastName { get; set; }
		public string WestName { get; set; }
		public int InitialPerspective { get; set; }

        public IEnumerator<string> GetEnumerator()
        {
            yield return NorthName;
            yield return SouthName;
            yield return EastName;
            yield return WestName;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}