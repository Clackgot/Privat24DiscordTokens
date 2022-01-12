namespace TokenBaseChekerCore
{
    /// <summary>
    /// Информация о загруженных токенах
    /// </summary>
    public struct TokensInfo
    {
        /// <summary>
        /// Удалено дублей
        /// </summary>
        public int RemovedDublicatesCount;

        /// <summary>
        /// Удалено "битых" строк
        /// </summary>
        public int RemovedBrokenLinesCount;


        /// <summary>
        /// Удалено пустых строк
        /// </summary>
        public int RemovedEmptyLinesCount;

        public override string ToString()
        {
            return $"Удалено дублей: {RemovedDublicatesCount} Удалено битых строк: {RemovedBrokenLinesCount} Удалено пустых строк: {RemovedEmptyLinesCount}";
        }
    }
}
