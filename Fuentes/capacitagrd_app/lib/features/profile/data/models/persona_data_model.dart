import 'package:equatable/equatable.dart';

// PersonaDataDTO / CrearPersonaDataDTO
class PersonaDataModel extends Equatable {
  final int idPersonaData;
  final int idPersona;
  final int idPaisLabora;
  final int idEntidad;
  final String nombreEntidadOtra;
  final String areaLabora;
  final int idCargo;
  final String nombreCargoOtra;
  final String idDistrito;
  final DateTime? fechaReg;

  const PersonaDataModel({
    this.idPersonaData = 0,
    required this.idPersona,
    required this.idPaisLabora,
    required this.idEntidad,
    required this.nombreEntidadOtra,
    required this.areaLabora,
    required this.idCargo,
    required this.nombreCargoOtra,
    required this.idDistrito,
    this.fechaReg,
  });

  factory PersonaDataModel.fromJson(Map<String, dynamic> j) => PersonaDataModel(
        idPersonaData: j['iD_PERSONA_DATA'] ?? 0,
        idPersona: j['iD_PERSONA'] ?? 0,
        idPaisLabora: j['iD_PAIS_LABORA'] ?? 0,
        idEntidad: j['iD_ENTIDAD'] ?? 0,
        nombreEntidadOtra: j['nombrE_ENTIDAD_OTRA'] ?? '',
        areaLabora: j['areA_LABORA'] ?? '',
        idCargo: j['iD_CARGO'] ?? 0,
        nombreCargoOtra: j['nombrE_CARGO_OTRA'] ?? '',
        idDistrito: j['iD_DISTRITO']?.toString() ?? '',
        fechaReg: j['fechA_REG'] != null ? DateTime.tryParse(j['fechA_REG']) : null,
      );

  Map<String, dynamic> toJson() => {
        'ID_PERSONA': idPersona,
        'ID_PAIS_LABORA': idPaisLabora,
        'ID_ENTIDAD': idEntidad,
        'NOMBRE_ENTIDAD_OTRA': nombreEntidadOtra,
        'AREA_LABORA': areaLabora,
        'ID_CARGO': idCargo,
        'NOMBRE_CARGO_OTRA': nombreCargoOtra,
        'ID_DISTRITO': int.tryParse(idDistrito) ?? 0,
      };

  @override
  List<Object?> get props => [idPersonaData, idPersona];
}
