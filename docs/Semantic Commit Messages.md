# Semantic Commit Messages

Kişisel projelerde kullanılmak üzere, dünya genelinde en yaygın kabul gören standart olan [Conventional Commits](https://www.conventionalcommits.org/) spesifikasyonuna dayalı commit mesajı kuralları.

## Genel Format

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

## Type Listesi

| Type | Kullanım Amacı |
|---|---|
| `feat` | Yeni bir özellik eklenmesi |
| `fix` | Bir hatanın düzeltilmesi |
| `docs` | Sadece dokümantasyon değişiklikleri |
| `style` | Kod davranışını etkilemeyen biçimlendirme (boşluk, noktalama, formatlama) |
| `refactor` | Ne hata düzeltme ne de yeni özellik olan kod değişikliği (yeniden yapılandırma) |
| `perf` | Performansı artıran bir kod değişikliği |
| `test` | Eksik testlerin eklenmesi veya mevcut testlerin düzeltilmesi |
| `build` | Build sistemi veya dış bağımlılıkları etkileyen değişiklikler (npm, nuget, docker vb.) |
| `ci` | CI/CD yapılandırma dosyaları ve script değişiklikleri |
| `chore` | Kaynak kodu veya testleri etkilemeyen diğer bakım işleri |
| `revert` | Önceki bir commit'i geri alma |

## Scope (İsteğe Bağlı)

Değişikliğin etkilediği alanı parantez içinde belirtir:

```
feat(auth): add refresh token support
fix(api): correct pagination offset calculation
docs(readme): update installation steps
```

## Breaking Change

Geriye dönük uyumsuz (breaking) bir değişiklik iki şekilde belirtilir:

1. Type/scope'tan sonra `!` eklenerek:
   ```
   feat(api)!: remove deprecated /v1/users endpoint
   ```

2. Footer'da `BREAKING CHANGE:` ile açıklanarak:
   ```
   feat(api): change response format of /orders endpoint

   BREAKING CHANGE: response artık "data" objesi yerine düz dizi döndürüyor.
   ```

## Örnekler

```
feat: add dark mode support

fix(auth): prevent token refresh race condition

docs: update contributing guidelines

refactor(order-service): extract pricing logic into separate class

perf(image-loader): lazy-load thumbnails to reduce initial payload

test(payment): add unit tests for discount calculation

chore(deps): bump Microsoft.EntityFrameworkCore to 9.0.1

fix(ui)!: remove legacy button component

BREAKING CHANGE: `LegacyButton` kaldırıldı, yerine `Button` kullanılmalı.
```

## Kurallar

1. **Type zorunludur**: Her commit mesajı yukarıdaki type listesinden biriyle başlamalı.
2. **Description kısa ve emir kipinde olmalı**: "added" değil "add", "fixed" değil "fix" (İngilizce yazılıyorsa emir kipi; Türkçe yazılıyorsa "ekle", "düzelt" gibi).
3. **İlk harf küçük, sonunda nokta yok**: `feat: Add login.` değil, `feat: add login`
4. **50 karakter kuralı**: Başlık satırı (description) ideal olarak 50, en fazla 72 karakteri geçmemeli.
5. **Body "ne" değil "neden" ve "nasıl" anlatmalı**: Kod zaten "ne" değiştiğini gösteriyor; body, gerekçeyi ve varsa alternatif yaklaşımları açıklamalı.
6. **Footer'da referans kullan**: İlgili issue/PR numaraları için `Closes #123`, `Refs #456` gibi ifadeler kullanılabilir.
7. **Bir commit, bir mantıksal değişiklik**: Birden fazla amacı tek commit'te birleştirme; her commit tek başına anlamlı ve geri alınabilir olmalı.
8. **Tutarlı dil kullan**: Tüm proje boyunca ya İngilizce ya da Türkçe description kullan, ikisini karıştırma.

## Neden Kullanılmalı?

- Otomatik `CHANGELOG` üretimi mümkün hale gelir (örn. `semantic-release`, `standard-version`).
- Commit geçmişinden semantic versioning (SemVer) otomatik çıkarılabilir: `fix` → PATCH, `feat` → MINOR, `BREAKING CHANGE` → MAJOR.
- Takım/topluluk genelinde okunabilirlik ve arama kolaylığı sağlar.

## Referanslar

- [Conventional Commits Specification](https://www.conventionalcommits.org/en/v1.0.0/)
- [Semantic Versioning](https://semver.org/)
