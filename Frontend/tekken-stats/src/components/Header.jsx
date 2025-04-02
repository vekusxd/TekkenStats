import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import styles from './TekkenStatsApp.module.css';
import { BASE_URL } from '../config/baseUrl';

const Header = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const navigate = useNavigate();

  const handleSearch = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.get(`${BASE_URL}/api/names`, {
        params: {
          StartsWith: searchQuery,
          Amount: 1
        }
      });
      
      if (response.data.length > 0) {
        navigate(`/${response.data[0].tekkenId}`);
      }
    } catch (err) {
      console.error('Search error:', err);
    }
  };

  return (
    <header className={styles.header}>
      <div className={styles.headerContainer}>
        <div className={styles.headerContent}>
          <a className={styles.logo} href="/">Tekken Stats</a>
          <div className={styles.headerSearchContainer}>
            <form onSubmit={handleSearch} className={styles.headerSearchForm}>
              <input 
                type="text" 
                placeholder="Search player..." 
                className={styles.headerSearchInput}
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
              />
              <button type="submit" className={styles.headerSearchIcon}>
                <svg 
                  xmlns="http://www.w3.org/2000/svg" 
                  viewBox="0 0 24 24" 
                  fill="none" 
                  stroke="currentColor" 
                  strokeWidth="2" 
                  strokeLinecap="round" 
                  strokeLinejoin="round"
                >
                  <circle cx="11" cy="11" r="8"></circle>
                  <path d="m21 21-4.3-4.3"></path>
                </svg>
              </button>
            </form>
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;