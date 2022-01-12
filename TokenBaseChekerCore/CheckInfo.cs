namespace TokenBaseChekerCore
{
    /// <summary>
    /// Результат проверки
    /// </summary>
    public struct CheckInfo
    {
        public TokensInfo dataInfo;
        public TokensInfo logsInfo;


        /// <summary>
        /// Токенов, которые уже есть в базе
        /// </summary>
        public int MatchesInDataCount;

        /// <summary>
        /// Новых токенов
        /// </summary>
        public int NewTokensInLogsCount;

        public override string ToString()
        {
            return $"База: {dataInfo}\n" +
                $"Логи: {logsInfo}\n" +
                $"Результат проверки: Найдено дублей: {MatchesInDataCount} Уникальных токенов: {NewTokensInLogsCount}";
        }

    }
}
