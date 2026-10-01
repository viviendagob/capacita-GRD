import 'package:equatable/equatable.dart';

class CuestionarioPreguntaRespuestaModel extends Equatable {
  final int idPreguntaRespuesta;
  final String nombre;
  final int idCuestionarioPregunta;

  const CuestionarioPreguntaRespuestaModel({
    required this.idPreguntaRespuesta,
    required this.nombre,
    required this.idCuestionarioPregunta,
  });

  factory CuestionarioPreguntaRespuestaModel.fromJson(Map<String, dynamic> j) =>
      CuestionarioPreguntaRespuestaModel(
        idPreguntaRespuesta: j['iD_PREGUNTA_RESPUESTA'] ?? 0,
        nombre: j['nombre'] ?? '',
        idCuestionarioPregunta: j['iD_CUESTIONARIO_PREGUNTA'] ?? 0,
      );

  @override
  List<Object?> get props => [idPreguntaRespuesta];
}

class CuestionarioPreguntaModel extends Equatable {
  final int idCuestionarioPregunta;
  final String nombre;
  final int peso;
  final int idCuestionario;
  final List<CuestionarioPreguntaRespuestaModel> respuestas;

  const CuestionarioPreguntaModel({
    required this.idCuestionarioPregunta,
    required this.nombre,
    required this.peso,
    required this.idCuestionario,
    this.respuestas = const [],
  });

  factory CuestionarioPreguntaModel.fromJson(Map<String, dynamic> j) => CuestionarioPreguntaModel(
        idCuestionarioPregunta: j['iD_CUESTIONARIO_PREGUNTA'] ?? 0,
        nombre: j['nombre'] ?? '',
        peso: j['peso'] ?? 0,
        idCuestionario: j['iD_CUESTIONARIO'] ?? 0,
        respuestas: (j['rESPUESTAS'] as List? ?? [])
            .map((r) => CuestionarioPreguntaRespuestaModel.fromJson(r))
            .toList(),
      );

  @override
  List<Object?> get props => [idCuestionarioPregunta];
}

class CuestionarioModel extends Equatable {
  final int idCuestionario;
  final String nombre;
  final String? userReg;
  final DateTime? fechaReg;
  final List<CuestionarioPreguntaModel> preguntas;

  const CuestionarioModel({
    required this.idCuestionario,
    required this.nombre,
    this.userReg,
    this.fechaReg,
    this.preguntas = const [],
  });

  int get puntajeTotal => preguntas.fold(0, (sum, p) => sum + p.peso);

  factory CuestionarioModel.fromJson(Map<String, dynamic> j) => CuestionarioModel(
        idCuestionario: j['iD_CUESTIONARIO'] ?? 0,
        nombre: j['nombre'] ?? '',
        userReg: j['useR_REG'],
        fechaReg: j['fechA_REG'] != null ? DateTime.tryParse(j['fechA_REG']) : null,
        preguntas: (j['pREGUNTAS'] as List? ?? []).map((p) => CuestionarioPreguntaModel.fromJson(p)).toList(),
      );

  @override
  List<Object?> get props => [idCuestionario];
}

// Respuesta local del participante al cuestionario
class RespuestaCuestionarioModel extends Equatable {
  final int idCuestionario;
  final int idPersona;
  final int idEvento;
  final int idPregunta;
  final int idRespuestaSeleccionada;
  final DateTime fechaRespuesta;

  const RespuestaCuestionarioModel({
    required this.idCuestionario,
    required this.idPersona,
    required this.idEvento,
    required this.idPregunta,
    required this.idRespuestaSeleccionada,
    required this.fechaRespuesta,
  });

  Map<String, dynamic> toJson() => {
        'idCuestionario': idCuestionario,
        'idPersona': idPersona,
        'idEvento': idEvento,
        'idPregunta': idPregunta,
        'idRespuestaSeleccionada': idRespuestaSeleccionada,
        'fechaRespuesta': fechaRespuesta.toIso8601String(),
      };

  @override
  List<Object?> get props => [idCuestionario, idPersona, idPregunta];
}

class ResultadoCuestionario extends Equatable {
  final int puntajeObtenido;
  final int puntajeTotal;
  final bool aprobado;
  final List<RespuestaCuestionarioModel> respuestas;

  const ResultadoCuestionario({
    required this.puntajeObtenido,
    required this.puntajeTotal,
    required this.aprobado,
    required this.respuestas,
  });

  double get porcentaje => puntajeTotal > 0 ? (puntajeObtenido / puntajeTotal) * 100 : 0;

  @override
  List<Object?> get props => [puntajeObtenido, puntajeTotal];
}
