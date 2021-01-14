namespace Trader.Domain.Model
{
    public class TransactionItem
    {
        public string Id { get; set; }//交易流水id

        public string UserId { get; set; }
        public string AccountNo { get; set; }//转出账户卡号
        public string BankNo { get; set; }//银行编码
        public decimal Amount { get; set; }// 交易金额
        public int Status { get; set; } // 交易状态 （待处理、处理成功、处理失败）
        public long LastUpdateTime { get; set; }//最后更新时间
        public string Comment { get; set; }//备注
    }
}
