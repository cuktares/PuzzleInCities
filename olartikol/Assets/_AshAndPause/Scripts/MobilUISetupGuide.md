# Mobil UI Kurulum Rehberi

## 🔧 Ana Sorun
Oyunun mobil uyumundaki ana sorun, **InputSystemWrapper** ile **StarterAssetsInputs** arasındaki bağlantı eksikliğiydi. Bu sorunu çözdük!

## 🚀 Çözüm
1. **InputSystemWrapper** scripti güncellendi ve artık mobil input'ları destekliyor
2. **MobileUIController** scripti iyileştirildi
3. **UICanvasControllerInput** scriptine eksik metodlar eklendi
4. **MobileInputTester** test scripti oluşturuldu

## 🎮 Sahne Kurulumu (Adım Adım)

### 1. Player GameObject'i Ayarlama
- Player GameObject'ine **InputSystemWrapper** scriptini ekleyin
- Player GameObject'ine **StarterAssetsInputs** scriptini ekleyin (yoksa)

### 2. UI Canvas Ayarlama
- `UI_Canvas_StarterAssetsInputs_Joysticks` prefab'ını sahneye ekleyin
- Canvas'taki **UICanvasControllerInput** scriptinde `starterAssetsInputs` referansını atayın

### 3. MobileUIController Ayarlama
- Boş bir GameObject oluşturun ve **MobileUIController** scriptini ekleyin
- `starterAssetsInputs` referansını atayın
- `mobileUICanvas` referansını atayın (UI_Canvas_StarterAssetsInputs_Joysticks)

### 4. UI Butonları Ayarlama
UI Canvas'taki butonları şu şekilde ayarlayın:

#### Jump Butonu:
- **UIVirtualButton** scriptinde:
  - `buttonStateOutputEvent` → `UICanvasControllerInput.VirtualJumpInput`

#### Interact Butonu (Grab):
- **UIVirtualButton** scriptinde:
  - `buttonStateOutputEvent` → `UICanvasControllerInput.OnGrabButtonPressed` (OnPointerDown)
  - `buttonStateOutputEvent` → `UICanvasControllerInput.OnGrabButtonReleased` (OnPointerUp)

#### Hold Butonu:
- **UIVirtualButton** scriptinde:
  - `buttonStateOutputEvent` → `UICanvasControllerInput.OnHoldButtonPressed` (OnPointerDown)
  - `buttonStateOutputEvent` → `UICanvasControllerInput.OnHoldButtonReleased` (OnPointerUp)

#### Clone Butonu:
- **UIVirtualButton** scriptinde:
  - `buttonStateOutputEvent` → `UICanvasControllerInput.OnCloneButtonPressed` (OnPointerDown)
  - `buttonStateOutputEvent` → `UICanvasControllerInput.OnCloneButtonReleased` (OnPointerUp)

#### Joystick:
- **UIVirtualJoystick** scriptinde:
  - `joystickOutputEvent` → `UICanvasControllerInput.VirtualMoveInput`

### 5. Test Etme
- **MobileInputTester** scriptini bir GameObject'e ekleyin
- Oyunu çalıştırın
- `M` tuşu ile mobil UI'ı açın/kapatın
- `T` tuşu ile test yapın
- Joystick ve butonların çalışıp çalışmadığını kontrol edin

## 🎯 Önemli Notlar

1. **Bağlantı Sırası**: MobileUIController → StarterAssetsInputs → InputSystemWrapper → PlayerController
2. **Mobil UI Aktivasyonu**: M tuşu ile mobil UI'ı açıp kapatabilirsiniz
3. **Test Modu**: T tuşu ile input testini yapabilirsiniz
4. **Debug Bilgileri**: MobileInputTester ekran üstünde debug bilgisi gösterir

## 🐛 Hata Ayıklama

### Joystick çalışmıyor:
- `UIVirtualJoystick.joystickOutputEvent` → `UICanvasControllerInput.VirtualMoveInput` bağlantısını kontrol edin

### Butonlar çalışmıyor:
- `UIVirtualButton.buttonStateOutputEvent` bağlantılarını kontrol edin
- `UICanvasControllerInput.starterAssetsInputs` referansını kontrol edin

### Mobil UI görünmüyor:
- `MobileUIController.mobileUICanvas` referansını kontrol edin
- M tuşu ile mobil UI'ı aktif edin

### Input geçmiyor:
- `InputSystemWrapper.starterAssetsInputs` referansını kontrol edin
- MobileUIController'ın Start() metodunda otomatik bağlantı kuruluyor

## 🎉 Sonuç
Artık mobil UI sistemi düzgün çalışmalı! Hem PC hem mobil input'lar aynı anda destekleniyor. 