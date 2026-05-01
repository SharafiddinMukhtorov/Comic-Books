// CoinService endi faqat Web layerdagi yordamchi — barcha logika Application layer Handlers da.
// Bu servis eski kodlar bilan mos kelish uchun saqlanadi.
// Yangi kodlarda to'g'ridan-to'g'ri Mediator.Send() ishlating.

namespace ComicBooks.Web.Services;

// Bu fayl bo'sh — CoinService o'rniga MediatR commands ishlatiladi:
// SpendCoinsCommand, AddCoinsCommand, GetCoinBalanceQuery, CheckChapterAccessQuery
