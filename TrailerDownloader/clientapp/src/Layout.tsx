import React from 'react';
import { AppBar, Toolbar, Typography, Container } from '@mui/material';

interface LayoutProps {
  children?: React.ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6">
            Movie Trailer Downloader
          </Typography>
        </Toolbar>
      </AppBar>
      <Container>
        {children}
      </Container>
    </>
  );
};

export default Layout;
