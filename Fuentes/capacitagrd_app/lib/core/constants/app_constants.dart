class AppConstants {
  // API Base URL - cambiar en producción
  // static const String baseUrl = 'http://10.0.2.2:5064'; // emulador Android
  static const String baseUrl = 'http://192.168.152.112:5064'; // red local (celular físico)
  // static const String baseUrl = 'https://api.capacitagrd.vivienda.gob.pe'; // producción

  static const String appName = 'CAPACITA-GRD';
  static const String appVersion = '1.0.0';

  // Hive boxes
  static const String authBox = 'auth_box';
  static const String eventsBox = 'events_box';
  static const String attendanceBox = 'attendance_box';
  static const String surveysBox = 'surveys_box';
  static const String questionnairesBox = 'questionnaires_box';
  static const String personaBox = 'persona_box';
  static const String pendingSyncBox = 'pending_sync_box';

  // Hive cache keys (dentro de cada box)
  static const String eventosBoxKey = 'mis_eventos';
  static const String encuestasBoxKey = 'respuestas_encuesta';
  static const String cuestionariosBoxKey = 'respuestas_cuestionario';

  // Secure storage keys
  static const String tokenKey = 'jwt_token';
  static const String tokenExpiryKey = 'token_expiry';
  static const String personaIdKey = 'persona_id';
  static const String personaDataIdKey = 'persona_data_id';
  static const String usernameKey = 'username';
  static const String passwordKey = 'password'; // solo si "recordar sesión" activo
  static const String biometricEnabledKey = 'biometric_enabled';
  static const String rolKey = 'rol'; // "ADMIN" (staff) o "PARTICIPANTE", viene del claim del JWT

  // Geo
  static const double geoRadioPermitidoMetros = 500.0;

  // Timeouts
  static const int connectTimeoutMs = 30000;
  static const int receiveTimeoutMs = 30000;
}

class ApiRoutes {
  // Auth
  static const String login = '/auth/login';

  // Personas
  static const String personasExiste = '/personas/existe';
  static const String personasObtener = '/personas'; // + /{id}
  static const String personasAgregar = '/personas';
  static const String personasActualizar = '/personas';

  // Eventos
  static const String eventos = '/eventos';
  static const String eventosEstadisticas = '/eventos/estadisticas';

  // Participantes
  static const String eventosParticipantes = '/eventosparticipantes';
  static const String misEventos = '/eventosparticipantes/eventos';

  // Asistencias
  static const String eventosAsistencias = '/eventosasistencias';

  // Encuestas
  static const String encuestas = '/encuestas';
  static const String eventoEncuestasResponder = '/eventoencuestas/responder';
  static const String eventoEncuestasCompletada = '/eventoencuestas/completada';

  // Cuestionarios
  static const String cuestionarios = '/cuestionarios';

  // PDF / Certificados
  static const String crearPdf = '/createpdf/crear';
  static const String certificados = '/certificados';

  // Persona Data
  static const String personas = '/personas';
  static const String personaData = '/personasdata';

  // Maestros
  static const String cargos = '/cargos';
  static const String profesiones = '/profesiones';
  static const String paises = '/paises';
  static const String distritos = '/distritos';
  static const String tiposDocumentos = '/tiposdocumentos';
  static const String estados = '/estados';
  static const String tiposEventos = '/tiposeventos';
  static const String entidades = '/entidades';
  static const String modalidades = '/maestroseventos'; // obtiene modalidades
  static const String configuracion = '/configuracion';
}
