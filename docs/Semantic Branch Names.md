# Semantic Branch Names

Kişisel projelerde tutarlılık sağlamak için topluluk genelinde kabul görmüş (Git Flow / GitHub Flow kökenli) branch isimlendirme standardı.

## Genel Format

```
<type>/<short-description>
```

- `type`: branch'in amacını belirten sabit bir anahtar kelime (aşağıdaki listeden)
- `short-description`: kısa, açıklayıcı, kebab-case (tire ile ayrılmış) bir özet

İsteğe bağlı olarak issue/ticket numarası eklenebilir:

```
<type>/<issue-no>-<short-description>
```

## Type Listesi

| Type | Kullanım Amacı |
|---|---|
| `feature/` | Yeni bir özellik geliştirme |
| `fix/` | Production'da olmayan, geliştirme sürecindeki bir hatanın düzeltilmesi |
| `hotfix/` | Production'daki acil/kritik bir hatanın düzeltilmesi |
| `release/` | Bir sürüm yayınına hazırlık (versiyon numarası, changelog vb.) |
| `chore/` | Kod davranışını değiştirmeyen bakım işleri (bağımlılık güncelleme, config vb.) |
| `docs/` | Sadece dokümantasyon değişiklikleri |
| `refactor/` | Davranışı değiştirmeyen kod iyileştirmesi/yeniden yapılandırma |
| `test/` | Test ekleme veya mevcut testleri düzenleme |
| `perf/` | Performans iyileştirmesi |
| `style/` | Kod formatlama, boşluk, noktalama gibi kozmetik değişiklikler |
| `build/` | Build sistemi veya dış bağımlılıklarla ilgili değişiklikler |
| `ci/` | CI/CD pipeline yapılandırma değişiklikleri |
| `revert/` | Önceki bir commit'i geri alma |

## Örnekler

```
feature/user-authentication
feature/42-payment-integration
fix/login-redirect-loop
hotfix/critical-null-reference-crash
release/v2.3.0
chore/update-dependencies
docs/api-usage-guide
refactor/extract-payment-service
test/add-order-service-unit-tests
perf/optimize-image-loading
ci/add-github-actions-pipeline
```

## Kurallar

1. **Küçük harf kullan**: Branch isimleri tamamen küçük harf olmalı (`Feature/Login` değil, `feature/login`).
2. **Kelimeleri tire (`-`) ile ayır**: Alt çizgi (`_`) veya camelCase kullanma.
3. **Kısa ve açıklayıcı ol**: İdeal olarak 2-5 kelime; branch'in ne yaptığını isminden anlamak mümkün olmalı.
4. **Boşluk ve özel karakter kullanma**: Sadece harf, rakam, tire ve `/` kullan.
5. **Bir branch, bir amaç**: Her branch tek bir mantıksal işi kapsamalı; birden fazla konuyu tek branch'te birleştirme.
6. **Ticket/issue referansı varsa ekle**: Takip edilebilirlik için `feature/42-payment-integration` gibi issue numarasını isme dahil et.
7. **`main`/`develop` gibi ana branch'lere doğrudan commit atma**: Her zaman bir feature/fix branch'i üzerinden çalış ve pull request ile birleştir.

## Referanslar

- [Git Flow (Vincent Driessen)](https://nvie.com/posts/a-successful-git-branching-model/)
- [GitHub Flow](https://docs.github.com/en/get-started/using-github/github-flow)
- [Conventional Branch](https://conventional-branch.github.io/)
