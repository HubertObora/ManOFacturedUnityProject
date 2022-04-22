using System.Collections.Generic;

namespace Skrypty.Extensions {
  /// <summary>
  /// Odpowiada za dodatkowe możliwości dla istniejących klas
  /// </summary>
  public static class Extensions {
    /// <summary>
    /// Pozwala na dekonstrukcję dwuelementowej listy stringów
    /// </summary>
    /// <param name="list">Lista stringów</param>
    /// <param name="first">Pierwszy dekonstruowany element</param>
    /// <param name="second">Drugi dekonstruowany element</param>
    public static void Deconstruct(this IList<string> list, out int first, out int second) {
      if (list.Count == 2) {
        if (!int.TryParse(list[0], out first)) {
          first = default;
        }

        if (!int.TryParse(list[1], out second)) {
          second = default;
        }
      } else {
        first = default;
        second = default;
      }
    }
  }
}