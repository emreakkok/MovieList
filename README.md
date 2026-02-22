# MovieList 🎬🍿

MovieList, sinemaseverlerin filmleri keşfedebileceği, izleme geçmişlerini kaydedebileceği, puanlayıp yorum yapabileceği ve diğer kullanıcıları takip edebileceği **sosyal bir film takip platformudur**. 

Bu proje, **.NET 8** ve **N-Tier (Katmanlı Mimari)** prensipleri kullanılarak geliştirilmiş olup, film verileri için gerçek zamanlı olarak **The Movie Database (TMDB) API** ile entegre çalışmaktadır.

## ✨ Öne Çıkan Özellikler

### 🎥 Kapsamlı Film Modülü (TMDB Entegrasyonu)
* **Gerçek Zamanlı Veri:** TMDB API üzerinden güncel popüler filmler, vizyondakiler ve arama sonuçları.
* **Film Etkileşimleri:** Filmleri "İzledim" olarak işaretleme, 1-10 arası puan verme, favorilere (en fazla 4 adet) ve Watchlist'e (İzleme Listesi) ekleme.
* **İnceleme Sistemi:** Sadece izlenmiş filmlere yorum (review) yapabilme kuralı ve karakter sınırlandırmalı dinamik yorum sistemi.

### 👥 Sosyal Ağ Özellikleri
* **Takip Sistemi (Follow/Unfollow):** Diğer kullanıcıları takip edebilme, takipçi ve takip edilen listelerini görüntüleme.
* **Kullanıcı Profilleri:** Kullanıcılara özel profil sayfaları (Favori filmler, son izlenenler, watchlist ve puanlanmış filmlerin vitrini).
* **Kullanıcı Arama:** AJAX tabanlı anlık kullanıcı arama modülü.

### 🔐 Güvenlik ve Kimlik Doğrulama
* **ASP.NET Core Identity & JWT:** Güvenli üyelik sistemi. Geleneksel Cookie kimlik doğrulamasının yanı sıra mimari olarak JWT (JSON Web Token) altyapısı da entegre edilmiştir.
* **Otomatik Profil Fotoğrafı:** Kayıt olan kullanıcılar için *UI Avatars API* ile rastgele renklerde isme özel profil görseli ataması.

### ⚡ Dinamik Kullanıcı Deneyimi
* Sayfa yenilenmeden çalışan AJAX tabanlı etkileşimler (Arama, Takip Etme, Puan Verme, Watchlist'e Ekleme).

## 🛠️ Kullanılan Teknolojiler ve Mimari

Proje, bağımlılıkların ayrıştırıldığı **4 Katmanlı (N-Tier)** bir mimari üzerine inşa edilmiştir.

* **Sunum (Presentation):** ASP.NET Core MVC, Bootstrap 5, FontAwesome, jQuery, AJAX
* **İş Katmanı (Business):** Servis sınıfları (AuthService, MovieService vb.), TMDB HttpClient entegrasyonu.
* **Veri Erişim (DataAccess):** Entity Framework Core 8, Repository Design Pattern, SQL Server
* **Çekirdek (Core):** Varlıklar (Entities), DTOs (Data Transfer Objects), Arayüzler (Interfaces)
