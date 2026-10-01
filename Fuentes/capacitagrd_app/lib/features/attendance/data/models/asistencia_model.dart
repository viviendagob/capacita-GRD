import 'package:equatable/equatable.dart';

class AsistenciaModel extends Equatable {
  final int idAsistencia;
  final int idEvento;
  final int idPersona;
  final int idModalidad;
  final DateTime fecha;

  const AsistenciaModel({
    required this.idAsistencia,
    required this.idEvento,
    required this.idPersona,
    required this.idModalidad,
    required this.fecha,
  });

  factory AsistenciaModel.fromJson(Map<String, dynamic> j) => AsistenciaModel(
        idAsistencia: j['iD_ASISTENCIA'] ?? 0,
        idEvento: j['iD_EVENTO'] ?? 0,
        idPersona: j['iD_PERSONA'] ?? 0,
        idModalidad: j['iD_MODALIDAD'] ?? 0,
        fecha: DateTime.tryParse(j['fecha'] ?? '') ?? DateTime.now(),
      );

  Map<String, dynamic> toJson() => {
        'ID_EVENTO': idEvento,
        'ID_PERSONA': idPersona,
        'ID_MODALIDAD': idModalidad,
        'FECHA': fecha.toIso8601String(),
      };

  @override
  List<Object?> get props => [idAsistencia, idEvento, idPersona, fecha];
}

class RegistrarAsistenciaRequest {
  final int idEvento;
  final int idPersona;
  final int idModalidad;
  final DateTime fecha;
  final double? latitud;
  final double? longitud;

  const RegistrarAsistenciaRequest({
    required this.idEvento,
    required this.idPersona,
    required this.idModalidad,
    required this.fecha,
    this.latitud,
    this.longitud,
  });

  Map<String, dynamic> toJson() => {
        'ID_EVENTO': idEvento,
        'ID_PERSONA': idPersona,
        'ID_MODALIDAD': idModalidad,
        'FECHA': fecha.toIso8601String(),
      };
}
