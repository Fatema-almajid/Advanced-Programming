using Microsoft.EntityFrameworkCore;
using TrainingCertificationPlatform.Models;

namespace TrainingCertificationPlatform.Services
{
    public class PaymentTrackingService
    {
        private  readonly AppDbContext _context;

        public PaymentTrackingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string?> RecordPaymentAsync(int enrollmentId, double paymentAmount) {
           

            if (paymentAmount <= 0) {
                return "Payment amount must be greater than zero";
            }

            var balance = await _context.Balances
               .FirstOrDefaultAsync(b => b.EnrollmentId == enrollmentId);

            if (balance == null)
            {
                return "Balance record not found";
            }

            if (balance.AmountDue <= 0) {
                return "This balance has already been fully paid";
            }

         

            if (paymentAmount > balance.AmountDue)
            {
                return $"Payment exceeds remaining balance of BHD {balance.AmountDue}.";
            }

            var remainingBalance = balance.AmountDue - paymentAmount;

            var payment = new Payment
            {
                EnrollmentId = enrollmentId,
                Amount = paymentAmount,
                PaymentDate = DateTime.Now,
                Status = remainingBalance == 0 ? PaymentStatus.FULL :PaymentStatus.PARTIAL
            };

     
                balance.AmountDue -= (int)paymentAmount;
                balance.Status = remainingBalance == 0 ? BalanceStatus.PAID : BalanceStatus.PENDIG;
            

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return null;
        }

        public async Task FlagOverdueBalancesAsync() { 
            var overdueBalances = await _context.Balances
                .Where(b => 
                b.AmountDue > 0  &&
                b.DueDate.Date < DateTime.Now &&
                b.Status != BalanceStatus.OVERRDUE)
                .ToListAsync();

            foreach (var balance in overdueBalances)
            {
                balance.Status = BalanceStatus.OVERRDUE;
            }

            await _context.SaveChangesAsync();
        }
    }
}
