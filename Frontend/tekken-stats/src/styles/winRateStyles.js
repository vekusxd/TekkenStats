export const getWinRateColor = (winRate) => {
    if (winRate >= 50) return '#10B981'; // g
    return '#EF4444'; // r
  };
  
  export const winRateStyles = {
    textGreen: {
      color: '#10B981'
    },
    textRed: {
      color: '#EF4444'
    },
    progressFillGreen: {
      backgroundColor: '#10B981'
    },
    progressFillRed: {
      backgroundColor: '#EF4444'
    }
  };