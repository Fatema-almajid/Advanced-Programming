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

        public async Task<string?> RecordPaymentAsync(int enrollmentId, decimal paymentAmount) { 
           

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


            balance.AmountDue = remainingBalance;
            balance.Status = remainingBalance == 0 ? BalanceStatus.PAID : BalanceStatus.PENDING;
            

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return null;
        }

        public async Task FlagOverdueBalancesAsync() { 
            var overdueBalances = await _context.Balances
                .Where(b => 
                b.AmountDue > 0  &&
                b.DueDate < DateTime.Now &&
                b.Status != BalanceStatus.OVERDUE)
                .ToListAsync();

            foreach (var balance in overdueBalances)
            {
                balance.Status = BalanceStatus.OVERDUE;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Enrollment>> GetTraineePaymentsAsync(int traineeId)
        {
            await FlagOverdueBalancesAsync();

            return await _context.Enrollments
                .Include(e => e.Session)
                .ThenInclude(s => s.Course)
                .Include(e => e.Balance)
                .Include(e => e.Payments)
                .Where(e => e.TraineeId == traineeId)
                .ToListAsync();
        }
    }
}
