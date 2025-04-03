import React from 'react';
import styles from '../../styles/NameHistory.module.css';

const NameHistory = ({ names }) => {
  const formatUTCDate = (dateString) => {
    const date = new Date(dateString);
    return `${date.getUTCDate()} ${date.toLocaleString('en-US', { month: 'short' })} ${date.getUTCFullYear()}`;
  };

  return (
    <div className={styles.sidebarSection}>
      <h2 className={styles.sectionTitle}>Name History</h2>
      <div className={styles.nameHistory}>
        {names.map((name, index) => (
          <NameItem key={index} name={name} formatUTCDate={formatUTCDate} />
        ))}
      </div>
    </div>
  );
};

const NameItem = ({ name, formatUTCDate }) => (
  <div className={styles.nameHistoryItem}>
    <p className={styles.name}>{name.playerName}</p>
    <div className={styles.date}>
      <svg 
        xmlns="http://www.w3.org/2000/svg" 
        width="24" 
        height="24" 
        viewBox="0 0 24 24" 
        fill="none" 
        stroke="currentColor" 
        strokeWidth="2" 
        strokeLinecap="round" 
        strokeLinejoin="round" 
        className={styles.calendarIcon}
      >
        <path d="M8 2v4"></path>
        <path d="M16 2v4"></path>
        <rect width="18" height="18" x="3" y="4" rx="2"></rect>
        <path d="M3 10h18"></path>
        <path d="M8 14h.01"></path>
        <path d="M12 14h.01"></path>
        <path d="M16 14h.01"></path>
        <path d="M8 18h.01"></path>
        <path d="M12 18h.01"></path>
        <path d="M16 18h.01"></path>
      </svg>
      {formatUTCDate(name.date)}
    </div>
  </div>
);

export default NameHistory;