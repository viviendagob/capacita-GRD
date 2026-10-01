import 'package:dio/dio.dart';
import '../../../../core/constants/app_constants.dart';
import '../models/persona_model.dart';
import '../models/persona_data_model.dart';

abstract class PersonaRemoteDataSource {
  Future<PersonaModel> getPersona(int idPersona);
  Future<PersonaModel?> buscarPorDocumento(String numDocumento);
  Future<bool> existePersona(String numDocumento);
  Future<PersonaModel> crearPersona(PersonaModel persona, PersonaDataModel personaData);
  Future<void> actualizarPersona(PersonaModel persona);
  Future<PersonaDataModel> getPersonaData(int idPersonaData);
  Future<void> actualizarPersonaData(PersonaDataModel personaData);
}

class PersonaRemoteDataSourceImpl implements PersonaRemoteDataSource {
  final Dio dio;
  const PersonaRemoteDataSourceImpl({required this.dio});

  @override
  Future<PersonaModel> getPersona(int idPersona) async {
    final response = await dio.get('${ApiRoutes.personas}/$idPersona');
    return PersonaModel.fromJson(response.data);
  }

  @override
  Future<PersonaModel?> buscarPorDocumento(String numDocumento) async {
    try {
      final response = await dio.get('${ApiRoutes.personas}/documento/$numDocumento');
      return PersonaModel.fromJson(response.data);
    } on DioException catch (e) {
      if (e.response?.statusCode == 404) return null;
      rethrow;
    }
  }

  @override
  Future<bool> existePersona(String numDocumento) async {
    try {
      final response = await dio.get('${ApiRoutes.personasExiste}/$numDocumento');
      return response.data == true;
    } on DioException catch (e) {
      if (e.response?.statusCode == 404) return false;
      rethrow;
    }
  }

  @override
  Future<PersonaModel> crearPersona(PersonaModel persona, PersonaDataModel personaData) async {
    final response = await dio.post(ApiRoutes.personas, data: persona.toJsonCrear(personaData: personaData));
    return PersonaModel.fromJson(response.data);
  }

  @override
  Future<void> actualizarPersona(PersonaModel persona) async {
    await dio.put('${ApiRoutes.personas}/${persona.idPersona}', data: {
      'ID_PERSONA': persona.idPersona,
      'NOMBRES': persona.nombres,
      'APELLIDO_PATERNO': persona.apellidoPaterno,
      'APELLIDO_MATERNO': persona.apellidoMaterno,
      'EMAIL': persona.email,
      'CELULAR': persona.celular,
      'ID_PROFESION': persona.idProfesion,
      'FECHA_NACIMIENTO': persona.fechaNacimiento?.toIso8601String(),
    });
  }

  @override
  Future<PersonaDataModel> getPersonaData(int idPersonaData) async {
    final response = await dio.get('${ApiRoutes.personaData}/$idPersonaData');
    return PersonaDataModel.fromJson(response.data);
  }

  @override
  Future<void> actualizarPersonaData(PersonaDataModel personaData) async {
    await dio.put('${ApiRoutes.personaData}/${personaData.idPersonaData}', data: personaData.toJson());
  }
}
