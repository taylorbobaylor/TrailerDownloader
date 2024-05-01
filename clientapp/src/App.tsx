import React from 'react';
import { AppBar, Toolbar, Typography, Container, Box } from '@mui/material';
import './App.css';

function App() {
  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            TrailerDownloader
          </Typography>
        </Toolbar>
      </AppBar>
      <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
        {/* Content goes here */}
        <Typography paragraph>
          Welcome to TrailerDownloader. Here you can manage and download movie trailers for your Plex library.
        </Typography>
      </Container>
      <Box component="footer" sx={{ bgcolor: 'background.paper', py: 6 }}>
        <Container maxWidth="lg">
          <Typography variant="body2" color="text.secondary" align="center">
            © {new Date().getFullYear()} TrailerDownloader
          </Typography>
        </Container>
      </Box>
    </Box>
  );
}

export default App;
