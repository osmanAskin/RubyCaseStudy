# Ruby

Renk eşleştirme mekaniği üzerine kurulu, konveyör bazlı bir mobil puzzle oyunu. Oyuncu, ızgara üzerindeki renkli küpleri temizlemek için CollectableBox'ları seçip konveyöre gönderiyor. CollectableBox'lar ilerlerken kendi renkleriyle eşleşen küpleri absorbe ediyor ve grid temizlenince seviye tamamlanıyor.

---

## Oynanış



https://github.com/user-attachments/assets/757fe0dc-bcb0-409f-b111-112fa9f62457



- Ekrandaki CollectableBox kutularından birine tıklayıp konveyöre gönderiyorsun
- Konveyörde ilerleyen CollectableBox, önüne gelen ve kendi rengiyle eşleşen küpleri çekiyor
- Bir CollectableBox tüm çekme kapasitesini doldurunca yok oluyor
- Tüm renkli küpler temizlenirse seviye kazanılıyor
- Shooter'lar konveyor boyunca farklı yönlere dönebiliyor, bu sayede grid'in farklı taraflarına bakıp küp alabiliyorlar

---

## Teknik Mimari

### State Machine
Oyun iki ayrı state machine üzerine kurulu:

- **MainStateMachine** — Oyun mantığını yönetiyor: `Start → Game → Finish`
- **UIStateMachine** — UI akışını yönetiyor: `Start → InGame → LevelEnd`

### Seviye Sistemi
Seviyeler ayrı scene'ler değil, ScriptableObject tabanlı `LevelData` asset'lerinden runtime'da üretiliyor. Her seviye bir texture'dan okunuyor; texture'ın piksel renkleri küp grid'ini oluşturuyor. Level editörü tamamen Odin Inspector üzerine kurulu, Unity inspector'dan texture atayıp grid boyutu ayarlayıp küpleri boyayabiliyorsun.

### Grid Sistemi
Üç ayrı grid var:

| Grid | Görev |
|---|---|
| ColorCubeGridSystem | Temizlenmesi gereken renkli küpler |
| CollectableBoxGridSystem | Konveyöre gönderilmeyi bekleyen CollectableBox'lar |
| ReservedSlotGridSystem | Atım hakkı bitmeden konveyordan çıkan CollectableBox'lar için bekleme alanı |

### Absorb Mekaniği
Shooter konveyörde ilerlerken her frame raycast atıyor. Raycast bir küpe çarparsa ve renkler eşleşiyorsa küp absorbe ediliyor. Küp fiziksel olarak CollectableBox'a doğru uçuyor, animasyon tamamlandıktan sonra hem sayaç güncelleniyor hem de shake animasyonu oynuyor. Birden fazla küp aynı anda havada uçabilir ama `_pendingAbsorbs` sayacı sayesinde kapasite aşımı olmuyor.

### Bağımlılık Yönetimi
`CoreInstaller` tüm sistem referanslarını `Awake`'de initialize ediyor (`DefaultExecutionOrder -100`). Event-driven iletişim için `GameEvents` kullanılıyor.

---

## Kullanılan Teknolojiler

| Araç | Kullanım Amacı |
|---|---|
| Unity (URP) | Render pipeline |
| DOTween Pro | Tüm animasyonlar |
| Dreamteck Splines | Konveyör yolu ve hareket sistemi |
| UniTask | Async/await operasyonları |
| Odin Inspector | Level editörü ve inspector araçları |
| New Input System | Dokunmatik ve mouse input yönetimi |

---

## Proje Yapısı

```
Assets/GameAssets/
├── Scripts/
│   ├── Core/          # Dependency injection, başlangıç setup'ı
│   ├── Data/          # GameSettings ScriptableObject
│   ├── Event/         # Oyun içi event sistemi
│   ├── Game/          # Oyun mekanikleri (CollectableBox, küp, konveyör, grid)
│   ├── Level/         # Level üretimi ve yönetimi
│   ├── Managers/      # GameManager, InputManager
│   ├── Pools/         # Object pooling sistemi
│   ├── SaveSystem/    # JSON tabanlı kayıt sistemi
│   ├── State Machine/ # Ana oyun ve UI state machine'leri
│   └── UI/            # UIGame, UIWin, UIFail sayfaları
├── Scenes/
│   └── GameScene.unity  # Tek sahne, her şey runtime'da üretiliyor
├── Scriptable/
│   ├── Settings/      # GameSettings asset'i
│   └── Levels/        # Level_1 - Level_4 asset'leri
└── Level Textures/    # Seviye grid'lerini oluşturan texture'lar
```

---

## Level Oluşturma

Yeni bir seviye eklemek için `LevelData` ScriptableObject'i oluşturup şu adımları takip etmek yeterli:

1. Texture ata — genişlik/yükseklik otomatik olarak küp grid boyutunu belirler
2. **Create Colors** butonuna bas — texture analiz edilip benzer renkler gruplandırılır
3. **Initialize Grid** — CollectableBox grid'i oluşturulur
4. Alt kısımdaki renk paletinden renk seç, grid hücrelerine tıklayarak CollectableBox'ları yerleştir
5. **Z** ile çekme kapasitesini artır, **X** ile azalt
6. **Check Shooter Values** ile toplam kapasitelerin küp sayılarıyla eşleştiğini doğrula

---

## Kayıt Sistemi

Oyuncu ilerleme bilgisi JSON formatında cihaza kaydediliyor. Şu an kaydedilen tek veri mevcut seviye numarası. Sistem interface tabanlı (`ISaveSystem`) yazıldığı için farklı bir kayıt yöntemiyle değiştirmek kolay.
