import 'dart:convert';

class JwtUtils {
  // El rol viaja en el claim estándar de .NET (ClaimTypes.Role), no como "role" simple.
  static const _roleClaimKey = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

  /// Decodifica el payload de un JWT sin validar la firma (solo para leer claims
  /// que ya llegaron autenticados desde la API; la validación real la hace el servidor).
  static Map<String, dynamic>? decodePayload(String token) {
    try {
      final parts = token.split('.');
      if (parts.length != 3) return null;
      var payload = parts[1];
      payload = payload.padRight((payload.length + 3) & ~3, '='); // base64url necesita padding
      final decoded = utf8.decode(base64Url.decode(payload));
      return jsonDecode(decoded) as Map<String, dynamic>;
    } catch (_) {
      return null;
    }
  }

  static String? extraerRol(String token) => decodePayload(token)?[_roleClaimKey] as String?;
}
