import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import '../../../../core/errors/failures.dart';
import '../../domain/repositories/eventos_repository.dart';
import '../datasources/eventos_local_datasource.dart';
import '../datasources/eventos_remote_datasource.dart';
import '../models/evento_model.dart';

class EventosRepositoryImpl implements EventosRepository {
  final EventosRemoteDataSource remote;
  final EventosLocalDataSource local;

  const EventosRepositoryImpl({required this.remote, required this.local});

  @override
  Future<Either<Failure, List<EventoParticipanteModel>>> getMisEventos(int idPersona) async {
    try {
      final eventos = await remote.getMisEventos(idPersona);
      await local.cacheEventos(eventos);
      return Right(eventos);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionError || e.type == DioExceptionType.receiveTimeout) {
        final cached = await local.getCachedEventos();
        if (cached.isNotEmpty) return Right(cached);
      }
      if (e.response?.statusCode == 401) return Left(UnauthorizedFailure());
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, List<EventoModel>>> getEventosDisponibles() async {
    try {
      final eventos = await remote.getEventosDisponibles();
      return Right(eventos);
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) return Left(UnauthorizedFailure());
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, EventoModel>> getEvento(int idEvento) async {
    try {
      final evento = await remote.getEvento(idEvento);
      return Right(evento);
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) return Left(UnauthorizedFailure());
      if (e.response?.statusCode == 404) return Left(NotFoundFailure('Evento no encontrado'));
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, EventoParticipanteModel>> inscribirseEvento({
    required int idEvento,
    required int idPersona,
    required int idPersonaData,
    required int idModalidad,
    double? latitud,
    double? longitud,
  }) async {
    try {
      final result = await remote.inscribirseEvento(
        idEvento: idEvento,
        idPersona: idPersona,
        idPersonaData: idPersonaData,
        idModalidad: idModalidad,
        latitud: latitud,
        longitud: longitud,
      );
      return Right(result);
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) return Left(UnauthorizedFailure());
      if (e.response?.statusCode == 409) return Left(ValidationFailure('Ya estás inscrito en este evento'));
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, EventoParticipanteModel?>> verificarInscripcion(int idEvento, int idPersona) async {
    try {
      final result = await remote.verificarInscripcion(idEvento, idPersona);
      return Right(result);
    } on DioException catch (e) {
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }
}
