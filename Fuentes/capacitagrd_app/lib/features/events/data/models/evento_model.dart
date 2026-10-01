import 'package:equatable/equatable.dart';

class TipoDocumentoModel extends Equatable {
  final int idTipoDocumento;
  final String nombre;
  const TipoDocumentoModel({required this.idTipoDocumento, required this.nombre});
  factory TipoDocumentoModel.fromJson(Map<String, dynamic> j) =>
      TipoDocumentoModel(idTipoDocumento: j['iD_TIPO_DOCUMENTO'] ?? 0, nombre: j['nombre'] ?? '');
  @override
  List<Object?> get props => [idTipoDocumento];
}

class EstadoModel extends Equatable {
  final int idEstado;
  final String nombre;
  const EstadoModel({required this.idEstado, required this.nombre});
  factory EstadoModel.fromJson(Map<String, dynamic> j) =>
      EstadoModel(idEstado: j['iD_ESTADO'] ?? 0, nombre: j['nombre'] ?? '');
  @override
  List<Object?> get props => [idEstado];
}

class DistritoModel extends Equatable {
  final String idDistrito;
  final String nombre;
  final String? departamento;
  final String? provincia;
  const DistritoModel({required this.idDistrito, required this.nombre, this.departamento, this.provincia});
  factory DistritoModel.fromJson(Map<String, dynamic> j) => DistritoModel(
        idDistrito: j['iD_DISTRITO']?.toString() ?? '',
        nombre: j['nombre'] ?? '',
        departamento: j['departamento'],
        provincia: j['provincia'],
      );
  @override
  List<Object?> get props => [idDistrito];
}

class ModalidadModel extends Equatable {
  final int idModalidad;
  final String nombre;
  const ModalidadModel({required this.idModalidad, required this.nombre});
  factory ModalidadModel.fromJson(Map<String, dynamic> j) =>
      ModalidadModel(idModalidad: j['iD_MODALIDAD'] ?? 0, nombre: j['nombre'] ?? '');
  @override
  List<Object?> get props => [idModalidad];
}

class TipoEventoModel extends Equatable {
  final int idTipoEvento;
  final String nombre;
  const TipoEventoModel({required this.idTipoEvento, required this.nombre});
  factory TipoEventoModel.fromJson(Map<String, dynamic> j) =>
      TipoEventoModel(idTipoEvento: j['iD_TIPO_EVENTO'] ?? 0, nombre: j['nombre'] ?? '');
  @override
  List<Object?> get props => [idTipoEvento];
}

class EventoFechaModel extends Equatable {
  final int idEvento;
  final DateTime fecha;
  final String? horaInicio;
  final String? horaFin;
  final String? esEncuesta;
  final int? idEncuesta;
  final String? esCuestionario;
  final int? idCuestionario;

  const EventoFechaModel({
    required this.idEvento,
    required this.fecha,
    this.horaInicio,
    this.horaFin,
    this.esEncuesta,
    this.idEncuesta,
    this.esCuestionario,
    this.idCuestionario,
  });

  factory EventoFechaModel.fromJson(Map<String, dynamic> j) => EventoFechaModel(
        idEvento: j['iD_EVENTO'] ?? 0,
        fecha: DateTime.tryParse(j['fecha'] ?? '') ?? DateTime.now(),
        horaInicio: j['horA_INICIO'],
        horaFin: j['horA_FIN'],
        esEncuesta: j['eS_ENCUESTA'],
        idEncuesta: j['iD_ENCUESTA'],
        esCuestionario: j['eS_CUESTIONARIO'],
        idCuestionario: j['iD_CUESTIONARIO'],
      );
  @override
  List<Object?> get props => [idEvento, fecha];
}

class EventoModel extends Equatable {
  final int idEvento;
  final String codEvento;
  final String nombre;
  final DateTime fechaInicio;
  final DateTime fechaFin;
  final String nombreLugar;
  final int idEstado;
  final EstadoModel? estado;
  final int idUbigeo;
  final DistritoModel? distrito;
  final String? banner;
  final int numParticipantes;
  final String descripcion;
  final String? enlaceWhatsapp;
  final String? horaInicio;
  final String? horaFin;
  final String? redSocial;
  final int idTipoEvento;
  final TipoEventoModel? tipoEvento;
  final int idModalidad;
  final ModalidadModel? modalidad;
  final int? idEncuesta;
  final int? idCuestionario;
  final String generaTicket;
  final String enviaCorreo;
  final String formato;
  final String requiereDocumento;
  final List<EventoFechaModel> fechas;

  const EventoModel({
    required this.idEvento,
    required this.codEvento,
    required this.nombre,
    required this.fechaInicio,
    required this.fechaFin,
    required this.nombreLugar,
    required this.idEstado,
    this.estado,
    required this.idUbigeo,
    this.distrito,
    this.banner,
    required this.numParticipantes,
    required this.descripcion,
    this.enlaceWhatsapp,
    this.horaInicio,
    this.horaFin,
    this.redSocial,
    required this.idTipoEvento,
    this.tipoEvento,
    required this.idModalidad,
    this.modalidad,
    this.idEncuesta,
    this.idCuestionario,
    required this.generaTicket,
    required this.enviaCorreo,
    required this.formato,
    required this.requiereDocumento,
    this.fechas = const [],
  });

  factory EventoModel.fromJson(Map<String, dynamic> j) => EventoModel(
        idEvento: j['iD_EVENTO'] ?? 0,
        codEvento: j['coD_EVENTO'] ?? '',
        nombre: j['nombre'] ?? '',
        fechaInicio: DateTime.tryParse(j['fechA_INICIO'] ?? '') ?? DateTime.now(),
        fechaFin: DateTime.tryParse(j['fechA_FIN'] ?? '') ?? DateTime.now(),
        nombreLugar: j['nombrE_LUGAR'] ?? '',
        idEstado: j['iD_ESTADO'] ?? 0,
        estado: j['eSTADO'] != null ? EstadoModel.fromJson(j['eSTADO']) : null,
        idUbigeo: j['iD_UBIGEO'] ?? 0,
        distrito: j['dISTRITO'] != null ? DistritoModel.fromJson(j['dISTRITO']) : null,
        banner: j['bANNER'],
        numParticipantes: j['nuM_PARTICIPANTES'] ?? 0,
        descripcion: j['descripcion'] ?? '',
        enlaceWhatsapp: j['enlacE_WHATSAPP'],
        horaInicio: j['horA_INICIO'],
        horaFin: j['horA_FIN'],
        redSocial: j['reD_SOCIAL'],
        idTipoEvento: j['iD_TIPO_EVENTO'] ?? 0,
        tipoEvento: j['tipO_EVENTO'] != null ? TipoEventoModel.fromJson(j['tipO_EVENTO']) : null,
        idModalidad: j['iD_MODALIDAD'] ?? 0,
        modalidad: j['mODALIDAD'] != null ? ModalidadModel.fromJson(j['mODALIDAD']) : null,
        idEncuesta: j['iD_ENCUESTA'],
        idCuestionario: j['iD_CUESTIONARIO'],
        generaTicket: j['generA_TICKET'] ?? 'N',
        enviaCorreo: j['enviA_CORREO'] ?? 'N',
        formato: j['formato'] ?? '',
        requiereDocumento: j['requierE_DOCUMENTO'] ?? 'N',
        fechas: (j['fECHAS'] as List? ?? []).map((f) => EventoFechaModel.fromJson(f)).toList(),
      );

  bool get isActivo => idEstado == 1;
  bool get isPresencial => idModalidad == 1;
  bool get isVirtual => idModalidad == 2;

  @override
  List<Object?> get props => [idEvento];
}

// EventoParticipanteDTO
class EventoParticipanteModel extends Equatable {
  final int idEvento;
  final int idPersonaData;
  final int idModalidad;
  final DateTime fechaReg;
  final double? latitud;
  final double? longitud;
  final int idPersona;
  final EventoModel? evento;
  final String? codigoQr;
  final String? qrUrl;

  const EventoParticipanteModel({
    required this.idEvento,
    required this.idPersonaData,
    required this.idModalidad,
    required this.fechaReg,
    this.latitud,
    this.longitud,
    required this.idPersona,
    this.evento,
    this.codigoQr,
    this.qrUrl,
  });

  factory EventoParticipanteModel.fromJson(Map<String, dynamic> j) => EventoParticipanteModel(
        idEvento: j['iD_EVENTO'] ?? 0,
        idPersonaData: j['iD_PERSONA_DATA'] ?? 0,
        idModalidad: j['iD_MODALIDAD'] ?? 0,
        fechaReg: DateTime.tryParse(j['fechA_REG'] ?? '') ?? DateTime.now(),
        latitud: (j['latitud'] as num?)?.toDouble(),
        longitud: (j['longitud'] as num?)?.toDouble(),
        idPersona: j['iD_PERSONA'] ?? 0,
        evento: j['evento'] != null ? EventoModel.fromJson(j['evento']) : null,
        codigoQr: j['codigO_QR'],
        qrUrl: j['qR_URL'],
      );

  Map<String, dynamic> toJson() => {
        'ID_EVENTO': idEvento,
        'ID_PERSONA_DATA': idPersonaData,
        'ID_MODALIDAD': idModalidad,
        'FECHA_REG': fechaReg.toIso8601String(),
        'LATITUD': latitud,
        'LONGITUD': longitud,
        'ID_PERSONA': idPersona,
      };

  @override
  List<Object?> get props => [idEvento, idPersona];
}
