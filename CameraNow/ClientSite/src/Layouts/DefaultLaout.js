import Footer from '../Components/Footer/Footer';
import Header from '../Components/Header/Header';

const DefaultLayout = ({ children }) => {
    return (
        <div>
            <Header />

            {children}

            <Footer />
        </div>
    );
};

export default DefaultLayout;
