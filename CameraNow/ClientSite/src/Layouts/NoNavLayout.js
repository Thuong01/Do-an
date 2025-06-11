import Footer from '../Components/Footer/Footer';
import Header from '../Components/Header/Header';

const NoNavLayout = ({ children }) => {
    return (
        <div>
            <Header />

            {children}

            <Footer />
        </div>
    );
};

export default NoNavLayout;
