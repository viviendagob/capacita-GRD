import 'package:equatable/equatable.dart';

class TipoEncuestaRespuestaModel extends Equatable {
  final int id;
  final String nombre;
  const TipoEncuestaRespuestaModel({required this.id, required this.nombre});
  factory TipoEncuestaRespuestaModel.fromJson(Map<String, dynamic> j) =>
      TipoEncuestaRespuestaModel(id: j['iD_TIPO_ENCUESTA_PREGUNTA'] ?? 0, nombre: j['nombre'] ?? '');

  // Tipos conocidos del sistema
  bool get esTextoLibre => nombre.toLowerCase().contains('texto') || nombre.toLowerCase().contains('abierta');
  bool get esOpcionUnica => nombre.toLowerCase().contains('única') || nombre.toLowerCase().contains('unica');
  bool get esOpcionMultiple => nombre.toLowerCase().contains('múltiple') || nombre.toLowerCase().contains('multiple');
  bool get esEscala => nombre.toLowerCase().contains('escala');
  bool get esSiNo => nombre.toLowerCase().contains('si/no') || nombre.toLowerCase().contains('sí/no');

  @override
  List<Object?> get props => [id];
}

class EncuestaRespuestaModel extends Equatable {
  final int idRespuesta;
  final String nombre;           // texto de la pregunta
  final String? respuestas;      // opciones separadas por ";" si aplica
  final int idEncuesta;
  final int idTipoEncuestaPregunta;
  final TipoEncuestaRespuestaModel? tipo;

  const EncuestaRespuestaModel({
    required this.idRespuesta,
    required this.nombre,
    this.respuestas,
    required this.idEncuesta,
    required this.idTipoEncuestaPregunta,
    this.tipo,
  });

  List<String> get opcionesLista =>
      (respuestas != null && respuestas!.isNotEmpty) ? respuestas!.split(';').map((e) => e.trim()).where((e) => e.isNotEmpty).toList() : [];

  factory EncuestaRespuestaModel.fromJson(Map<String, dynamic> j) => EncuestaRespuestaModel(
        idRespuesta: j['iD_RESPUESTA'] ?? 0,
        nombre: j['nombre'] ?? '',
        respuestas: j['respuestas'],
        idEncuesta: j['iD_ENCUESTA'] ?? 0,
        idTipoEncuestaPregunta: j['iD_TIPO_ENCUESTA_PREGUNTA'] ?? 0,
        tipo: j['tipO_ENCUESTA_PREGUNTA'] != null ? TipoEncuestaRespuestaModel.fromJson(j['tipO_ENCUESTA_PREGUNTA']) : null,
      );

  @override
  List<Object?> get props => [idRespuesta];
}

class EncuestaModel extends Equatable {
  final int idEncuesta;
  final String nombre;
  final String? userReg;
  final DateTime? fechaReg;
  final List<EncuestaRespuestaModel> preguntas;

  const EncuestaModel({
    required this.idEncuesta,
    required this.nombre,
    this.userReg,
    this.fechaReg,
    this.preguntas = const [],
  });

  factory EncuestaModel.fromJson(Map<String, dynamic> j) => EncuestaModel(
        idEncuesta: j['iD_ENCUESTA'] ?? 0,
        nombre: j['nombre'] ?? '',
        userReg: j['useR_REG'],
        fechaReg: j['fechA_REG'] != null ? DateTime.tryParse(j['fechA_REG']) : null,
        preguntas: (j['rESPUESTAS'] as List? ?? []).map((r) => EncuestaRespuestaModel.fromJson(r)).toList(),
      );

  @override
  List<Object?> get props => [idEncuesta];
}

// Respuesta local del participante (almacenada offline)
class RespuestaParticipanteModel extends Equatable {
  final int idEncuesta;
  final int idPersona;
  final int idEvento;
  final int idPregunta;
  final String respuesta;
  final DateTime fechaRespuesta;

  const RespuestaParticipanteModel({
    required this.idEncuesta,
    required this.idPersona,
    required this.idEvento,
    required this.idPregunta,
    required this.respuesta,
    required this.fechaRespuesta,
  });

  Map<String, dynamic> toJson() => {
        'idEncuesta': idEncuesta,
        'idPersona': idPersona,
        'idEvento': idEvento,
        'idPregunta': idPregunta,
        'respuesta': respuesta,
        'fechaRespuesta': fechaRespuesta.toIso8601String(),
      };

  // Nombres de campo tal como los espera CrearEventoEncuestaRespuestaDTO en la API
  // (el binding de ASP.NET Core es case-insensitive pero no ignora guiones bajos).
  Map<String, dynamic> toApiJson() => {
        'ID_EVENTO': idEvento,
        'ID_PERSONA': idPersona,
        'ID_ENCUESTA': idEncuesta,
        'ID_PREGUNTA': idPregunta,
        'RESPUESTA': respuesta,
      };

  factory RespuestaParticipanteModel.fromJson(Map<String, dynamic> j) => RespuestaParticipanteModel(
        idEncuesta: j['idEncuesta'] ?? 0,
        idPersona: j['idPersona'] ?? 0,
        idEvento: j['idEvento'] ?? 0,
        idPregunta: j['idPregunta'] ?? 0,
        respuesta: j['respuesta'] ?? '',
        fechaRespuesta: DateTime.tryParse(j['fechaRespuesta'] ?? '') ?? DateTime.now(),
      );

  @override
  List<Object?> get props => [idEncuesta, idPersona, idPregunta];
}
