import 'package:dio/dio.dart';
import '../../core/constants/app_constants.dart';

// Item genérico id + nombre, usado por Países, Entidades, Cargos y Profesiones.
class MaestroItem {
  final int id;
  final String nombre;
  const MaestroItem({required this.id, required this.nombre});

  factory MaestroItem.fromJson(Map<String, dynamic> j, {required String idKey}) => MaestroItem(
        id: j[idKey] ?? 0,
        nombre: j['nombre'] ?? '',
      );
}

class PaisItem extends MaestroItem {
  final bool esLocal;
  const PaisItem({required super.id, required super.nombre, required this.esLocal});

  factory PaisItem.fromJson(Map<String, dynamic> j) => PaisItem(
        id: j['iD_PAIS'] ?? 0,
        nombre: j['nombre'] ?? '',
        esLocal: j['eslocal'] == '1' || j['eslocal'] == 1,
      );
}

class DistritoItem {
  final int idDistrito;
  final String departamento;
  final String provincia;
  final String distrito;
  const DistritoItem({
    required this.idDistrito,
    required this.departamento,
    required this.provincia,
    required this.distrito,
  });

  factory DistritoItem.fromJson(Map<String, dynamic> j) => DistritoItem(
        idDistrito: j['iD_DISTRITO'] ?? 0,
        departamento: j['departamento'] ?? '',
        provincia: j['provincia'] ?? '',
        distrito: j['distrito'] ?? '',
      );
}

abstract class MaestrosRemoteDataSource {
  Future<List<PaisItem>> getPaises();
  Future<List<MaestroItem>> getEntidades();
  Future<List<MaestroItem>> getCargos();
  Future<List<MaestroItem>> getProfesiones();
  Future<List<DistritoItem>> getDistritos();
  Future<List<String>> getAreasLaborales();
  Future<MaestroItem> crearCargo(String nombre);
  Future<MaestroItem> crearProfesion(String nombre);
}

class MaestrosRemoteDataSourceImpl implements MaestrosRemoteDataSource {
  final Dio dio;
  const MaestrosRemoteDataSourceImpl({required this.dio});

  @override
  Future<List<PaisItem>> getPaises() async {
    final response = await dio.get(ApiRoutes.paises);
    return (response.data as List).map((e) => PaisItem.fromJson(e)).toList();
  }

  @override
  Future<List<MaestroItem>> getEntidades() async {
    final response = await dio.get(ApiRoutes.entidades);
    return (response.data as List).map((e) => MaestroItem.fromJson(e, idKey: 'iD_ENTIDAD')).toList();
  }

  @override
  Future<List<MaestroItem>> getCargos() async {
    final response = await dio.get(ApiRoutes.cargos);
    return (response.data as List).map((e) => MaestroItem.fromJson(e, idKey: 'iD_CARGO')).toList();
  }

  @override
  Future<List<MaestroItem>> getProfesiones() async {
    final response = await dio.get(ApiRoutes.profesiones);
    return (response.data as List).map((e) => MaestroItem.fromJson(e, idKey: 'iD_PROFESION')).toList();
  }

  @override
  Future<List<DistritoItem>> getDistritos() async {
    final response = await dio.get(ApiRoutes.distritos);
    return (response.data as List).map((e) => DistritoItem.fromJson(e)).toList();
  }

  @override
  Future<List<String>> getAreasLaborales() async {
    final response = await dio.get('${ApiRoutes.personaData}/areas');
    return (response.data as List).map((e) => e.toString()).toList();
  }

  @override
  Future<MaestroItem> crearCargo(String nombre) async {
    final response = await dio.post(ApiRoutes.cargos, data: {'NOMBRE': nombre});
    return MaestroItem.fromJson(response.data, idKey: 'iD_CARGO');
  }

  @override
  Future<MaestroItem> crearProfesion(String nombre) async {
    final response = await dio.post(ApiRoutes.profesiones, data: {'NOMBRE': nombre});
    return MaestroItem.fromJson(response.data, idKey: 'iD_PROFESION');
  }
}
