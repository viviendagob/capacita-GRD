# CAPACITA-GRD App - Guía de Configuración

## Prerrequisitos
- Flutter 3.x (`flutter --version`)
- Android Studio + Android SDK (API 21+)
- Xcode 15+ para iOS

## 1. Instalar dependencias
```bash
cd capacitagrd_app
flutter pub get
```

## 2. Configurar la URL de la API

Editar `lib/core/constants/app_constants.dart`:
- **Emulador Android**: `http://10.0.2.2:5064` (ya configurado)
- **Dispositivo físico en red local**: `http://192.168.X.X:5064`
- **Producción**: `https://api.capacitagrd.vivienda.gob.pe`

## 3. Firebase (Push Notifications)

1. Crear proyecto en [console.firebase.google.com](https://console.firebase.google.com)
2. Agregar app Android (package: `pe.gob.vivienda.capacitagrd`)
3. Descargar `google-services.json` → colocar en `android/app/`
4. Agregar app iOS → descargar `GoogleService-Info.plist` → colocar en `ios/Runner/`

Sin Firebase, la app funciona igual (las notificaciones push simplemente no funcionan).

## 4. Google Maps

Reemplazar `TU_GOOGLE_MAPS_API_KEY_AQUI` en `android/app/src/main/AndroidManifest.xml`
con tu API key de [Google Cloud Console](https://console.cloud.google.com).

Para iOS, agregar en `ios/Runner/AppDelegate.swift`:
```swift
GMSServices.provideAPIKey("TU_API_KEY")
```

## 5. Assets

Colocar en `assets/images/`:
- `logo_mvcs.png` — logo del ministerio

## 6. Ejecutar

```bash
# Emulador Android
flutter run

# Dispositivo físico
flutter run --release

# Build APK para distribuir
flutter build apk --release
# El APK estará en: build/app/outputs/flutter-apk/app-release.apk

# Build para iOS (requiere Mac + Xcode)
flutter build ipa
```

## 7. Distribuir APK a participantes

**Opción A — Google Drive/WhatsApp:**
1. `flutter build apk --release`
2. Compartir `app-release.apk` por WhatsApp o correo
3. El participante debe activar "Instalar fuentes desconocidas" en Android

**Opción B — Firebase App Distribution:**
1. Configurar Firebase App Distribution
2. `flutter build apk --release`
3. Subir APK desde Firebase Console
4. Invitar participantes por email

**Opción C — Google Play Store (producción):**
1. `flutter build appbundle --release`
2. Subir a Play Console

## 8. Credenciales del API (Login)
- El username es el **número de DNI** del participante
- La contraseña inicial es también el **número de DNI**
- El participante debe cambiarla desde el Admin Web

## Notas
- Los endpoints de respuestas a encuestas/cuestionarios no existen en el API actual.
  Las respuestas se guardan localmente (Hive) hasta que el API sea extendido.
- JWT expira en 24 horas — la app redirige al login automáticamente cuando expira.
