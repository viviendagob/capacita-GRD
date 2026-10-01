import 'package:equatable/equatable.dart';
import '../../data/models/persona_data_model.dart';

// PersonaDTO
class PersonaModel extends Equatable {
  final int idPersona;
  final String codPersona;
  final int idTipoDocumento;
  final String numDocumento;
  final String nombres;
  final String apellidoPaterno;
  final String apellidoMaterno;
  final String sexo;
  final int idPaisNacimiento;
  final DateTime? fechaNacimiento;
  final DateTime? fechaReg;
  final DateTime? fechaUpd;
  final String? email;
  final String? celular;
  final int? idProfesion;
  final String validadoPide;

  const PersonaModel({
    required this.idPersona,
    required this.codPersona,
    required this.idTipoDocumento,
    required this.numDocumento,
    required this.nombres,
    required this.apellidoPaterno,
    required this.apellidoMaterno,
    required this.sexo,
    required this.idPaisNacimiento,
    this.fechaNacimiento,
    this.fechaReg,
    this.fechaUpd,
    this.email,
    this.celular,
    this.idProfesion,
    required this.validadoPide,
  });

  String get nombreCompleto => '$nombres $apellidoPaterno $apellidoMaterno'.trim();
  bool get validado => validadoPide == 'S';

  factory PersonaModel.fromJson(Map<String, dynamic> j) => PersonaModel(
        idPersona: j['iD_PERSONA'] ?? 0,
        codPersona: j['coD_PERSONA'] ?? '',
        idTipoDocumento: j['iD_TIPO_DOCUMENTO'] ?? 0,
        numDocumento: j['nuM_DOCUMENTO'] ?? '',
        nombres: j['nombres'] ?? '',
        apellidoPaterno: j['apellidO_PATERNO'] ?? '',
        apellidoMaterno: j['apellidO_MATERNO'] ?? '',
        sexo: j['sexo'] ?? '',
        idPaisNacimiento: j['iD_PAIS_NACIMIENTO'] ?? 0,
        fechaNacimiento: j['fechA_NACIMIENTO'] != null ? DateTime.tryParse(j['fechA_NACIMIENTO']) : null,
        fechaReg: j['fechA_REG'] != null ? DateTime.tryParse(j['fechA_REG']) : null,
        fechaUpd: j['fechA_UPD'] != null ? DateTime.tryParse(j['fechA_UPD']) : null,
        email: j['email'],
        celular: j['celular'],
        idProfesion: j['iD_PROFESION'],
        validadoPide: j['validadO_PIDE'] ?? 'N',
      );

  Map<String, dynamic> toJsonCrear({required PersonaDataModel personaData}) => {
        'ID_TIPO_DOCUMENTO': idTipoDocumento,
        'NUM_DOCUMENTO': numDocumento,
        'NOMBRES': nombres,
        'APELLIDO_PATERNO': apellidoPaterno,
        'APELLIDO_MATERNO': apellidoMaterno,
        'SEXO': sexo,
        'ID_PAIS_NACIMIENTO': idPaisNacimiento,
        'FECHA_NACIMIENTO': fechaNacimiento?.toIso8601String(),
        'EMAIL': email,
        'CELULAR': celular,
        'ID_PROFESION': idProfesion,
        'VALIDADO_PIDE': validadoPide,
        'CrearPersonaDataDTO': personaData.toJson(),
      };

  PersonaModel copyWith({
    String? nombres,
    String? apellidoPaterno,
    String? apellidoMaterno,
    String? email,
    String? celular,
    int? idProfesion,
    DateTime? fechaNacimiento,
  }) =>
      PersonaModel(
        idPersona: idPersona,
        codPersona: codPersona,
        idTipoDocumento: idTipoDocumento,
        numDocumento: numDocumento,
        nombres: nombres ?? this.nombres,
        apellidoPaterno: apellidoPaterno ?? this.apellidoPaterno,
        apellidoMaterno: apellidoMaterno ?? this.apellidoMaterno,
        sexo: sexo,
        idPaisNacimiento: idPaisNacimiento,
        fechaNacimiento: fechaNacimiento ?? this.fechaNacimiento,
        fechaReg: fechaReg,
        fechaUpd: fechaUpd,
        email: email ?? this.email,
        celular: celular ?? this.celular,
        idProfesion: idProfesion ?? this.idProfesion,
        validadoPide: validadoPide,
      );

  @override
  List<Object?> get props => [idPersona, numDocumento];
}
