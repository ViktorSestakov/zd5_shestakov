using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace zd4_shestakov
{
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
            // Инициализация слайдера процентной ставки
            rateSlider.ValueChanged += (sender, e) =>
            {
                rateLabel.Text = $"{e.NewValue:F0}%";
            };
        }

        public void SetWelcomeMessage(string username)
        {
            welcomeLabel.Text = $"Welcome, {username}!";
            welcomeLabel.IsVisible = true;
        }

        private void OnCalculateClicked(object sender, EventArgs e)
        {
            if (!double.TryParse(amountEntry.Text, out double amount) || amount <= 0)
            {
                DisplayAlert("Ошибка", "Введите корректную сумму кредита", "OK");
                return;
            }

            if (!int.TryParse(termEntry.Text, out int term) || term <= 0)
            {
                DisplayAlert("Ошибка", "Введите корректный срок кредита", "OK");
                return;
            }
            double rate = rateSlider.Value / 100 / 12; // Месячная ставка
            string paymentType = paymentTypePicker.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(paymentType))
            {
                DisplayAlert("Ошибка", "Выберите тип платежа", "OK");
                return;
            }
            double monthlyPayment = 0;
            double totalPayment = 0;

            if (paymentType == "Аннуитетный")
            {
                // Расчет аннуитетного платежа
                monthlyPayment = amount * (rate * Math.Pow(1 + rate, term)) / (Math.Pow(1 + rate, term) - 1);
                totalPayment = monthlyPayment * term;
                monthlyPaymentLabel.Text = $"{monthlyPayment:F2} ₽";
            }
            else
            {
                // Расчет дифференцированного платежа (скрываем ежемесячный платеж)
                monthlyPaymentLabel.Text = "N/A";
                // Расчет общей суммы для дифференцированного платежа
                double principalPayment = amount / term;
                double totalInterest = 0;
                for (int i = 0; i < term; i++)
                {
                    double remainingAmount = amount - (principalPayment * i);
                    totalInterest += remainingAmount * rate;
                }

                totalPayment = amount + totalInterest;
            }
            totalPaymentLabel.Text = $"{totalPayment:F2} ₽";
            overpaymentLabel.Text = $"{(totalPayment - amount):F2} ₽";
            resultFrame.IsVisible = true;
        }
    }
}
